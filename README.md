# Sistema de Gestión de Bares - Tucumán 

Este proyecto es una plataforma Full Stack desarrollada con **.NET 10** y **React**, diseñada para automatizar la obtención, procesamiento y gestión de información sobre bares y eventos en la provincia de Tucumán. 

El sistema cumple con el ciclo completo de vida de los datos: extracción (mock/scraping), procesamiento inteligente (IA), almacenamiento persistente (CRUD) y visualización interactiva mediante un Dashboard.

---

## Características Principales

* **Automatización en Segundo Plano:** Un *Worker Service* (`BackgroundService`) se encarga de ejecutar la ingesta de datos de forma periódica e ininterrumpida.
* **Inteligencia Artificial (Gemini):** Integración directa con las APIs de Google Gemini para procesar, limpiar y enriquecer los datos.
* **Gestión Completa (CRUD):** Endpoints RESTful protegidos por validaciones robustas y borrado lógico (*Soft Delete*).
* **UI/UX Moderna:** Interfaz responsiva desarrollada en React con métricas en tiempo real, gráficos (Recharts) y control de estados modales.
* **Historial de Logs:** Trazabilidad completa de las ejecuciones del worker en la base de datos.

---

## Arquitectura y Patrones de Diseño

El backend fue diseñado siguiendo patrones de diseño y modularidad:

* **Inyección de Dependencias (DI):** Utilizada transversalmente para inyectar contextos de base de datos, servicios de IA y *Providers*.
* **Patrón Provider / Repository (`IBarProvider`):** Abstrae el origen de los datos. Actualmente lee un JSON (mock), pero permite conectar un Web Scraper real en el futuro sin modificar la lógica de negocio.
* **Data Transfer Objects (DTO):** Se utilizan clases específicas (`CrearBarDto`, `ActualizarBarDto`) para aislar el modelo de dominio (`Bar.cs`) de la capa de presentación.
* **Validación Fluida (FluentValidation):** Las reglas de negocio y validación de los DTOs están centralizadas y separadas de los controladores.
* **Soft Delete:** Los registros no se borran físicamente de la base de datos, asegurando la integridad de los datos estadísticos, sino que se marcan con un flag (`IsActive = false`).
* **Paginación en Backend:** Los datos se sirven paginados (`page`, `pageSize`) para optimizar la transferencia de red y el rendimiento del frontend.

---

## Integración con Inteligencia Artificial (Prompting & Tokenización)

Se utilizó el modelo **Gemini 1.5/2.5 Flash** a través de solicitudes HTTP nativas en el `BarSyncService`. La IA cumple un rol fundamental estructurando prompts con instrucciones específicas (Zero-Shot Prompting):

1. **Clasificación Semántica:** Recibe el nombre y una categoría genérica, y obliga a la IA a encasillarlo estrictamente en un `enum` válido del sistema (ej: "Pubs", "Cervecerias").
2. **Deduplicación Lógica:** La IA actúa como árbitro. Recibe el listado de la DB y el nuevo bar. A través del contexto evalúa variaciones de texto (Ej: *"Bar Irlanda Tucumán"* vs *"Irlanda Bar"*) y responde estrictamente con 'SI' o 'NO' para autorizar o bloquear el ingreso.
3. **Enriquecimiento de Datos:** Genera descripciones de marketing cortas (máx 2 líneas) utilizando un rol asignado: *"Sos un experto en turismo de Tucumán..."*.

---

## Tecnologías Utilizadas

**Backend:**
* C# / .NET 10 (Web API & Worker Service)
* Entity Framework Core 10
* PostgreSQL (Npgsql)
* FluentValidation
* xUnit & NSubstitute (Testing)

**Frontend:**
* React 19 + Vite
* Bootstrap 5.3 (Estilos base y utilidades)
* Recharts (Visualización de datos)
* React Icons

---

## Ejecución Local (Mini-Tutorial)

### Requisitos Previos
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Node.js](https://nodejs.org/) (v18 o superior)
* [PostgreSQL](https://www.postgresql.org/) (Instalado y en ejecución)
* Una [API Key de Google Gemini](https://aistudio.google.com/)

### 1. Configurar y Ejecutar el Backend

1. Navega a la carpeta del proyecto API:
   ```bash
   cd backend/BaresTucuman/BaresTucuman.API
2. Abre el archivo appsettings.json o appsettings.Development.json y configura tus variables esenciales:

  ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=BaresTucumanDB;Username=TU_USUARIO;Password=TU_CONTRASEÑA"
      },
      "Gemini": {
        "ApiKey": "TU_API_KEY_DE_GEMINI_AQUI"
      },
      "SyncWorker": {
        "IntervalMinutes": 2
      }
    }
  ```
3. Aplica las migraciones para crear la base de datos en PostgreSQL:
   ```bash
   dotnet ef database update
4. Ejecuta el servidor
   ```bash
   dotnet run
(La API se levantará en https://localhost:44312 o el puerto configurado en tus properties).


### 2. Configurar y ejecutar el Frontend

1. Abre una nueva terminal y navega a la carpeta del frontend:
   ```bash
   cd frontend
2. Instala las dependencias:
   ```bash
   npm install
3. Verifica que la URL del backend en (`src/api/baresApi.js`) coincida con el puerto donde se levantó tu API de .NET.
4. Ejecuta el entorno de desarrollo:
   ```bash
   npm run dev
5. Abre el navegador en `http://localhost:5173` o en el puerto correspondiente.

## Parte 5: Criterio Técnico (Respuestas a la consigna)

**¿Cómo evitás duplicados?**
Se implementó una doble validación:
1. Al sincronizar automáticamente, la IA evalúa si el nuevo bar es una variante de uno existente basándose en el nombre y ubicación. 
2. Como mecanismo de respaldo (fallback) y para las cargas manuales, se utiliza un algoritmo que normaliza cadenas y busca intersección de palabras clave ignorando términos comunes ("bar", "resto", "el", "la").

**¿Cómo escalarías este sistema?**
1. **Desacoplamiento:** Separaría el `BarSyncWorker` en un microservicio independiente (ej. Azure Functions o un contenedor Worker) para que el scraping intensivo no afecte el rendimiento de la API principal.
2. **Caché:** Implementaría Redis para almacenar los resultados de estadísticas y paginación, reduciendo las consultas a la base de datos PostgreSQL.
3. **Colas de Mensajes:** Si los datos fueran masivos, enviaría los bares scrapeados a un sistema de colas (RabbitMQ/Kafka) para que la IA los procese asíncronamente sin bloqueos.

**¿Qué problemas puede tener este flujo?**
1. **Rate Limits de la IA:** Si el origen de datos envía cientos de bares de golpe, la API de Gemini podría limitar las peticiones (más si se tiene una apikey gratuita que su limite son de 15 peticiones por minuto en el caso de gemini), provocando fallos en la sincronización. (Se mitigó parcialmente añadiendo un `Task.Delay`, pero en gran escala requiere colas).
2. **Falsos positivos:** La IA podría clasificar erróneamente un bar o asumir que dos sucursales de la misma franquicia son un duplicado.

**¿Cómo mejorarías la calidad de los datos?**
1. Integrando la API de Google Maps o Mapbox para estandarizar las direcciones y obtener coordenadas (Lat/Lng) reales.
2. Implementando un sistema de estado "Pendiente de Aprobación" donde un operador humano pueda verificar la información traída por el bot antes de ser visible al público.
3. Extrayendo y guardando imágenes de referencia de los lugares.

---

## Demostración del Flujo

A continuación se muestra el ciclo de vida de la automatización y el uso de la plataforma:

**1. Estado Inicial:** Base de datos vacía al iniciar el sistema.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/8adf27d4-7a53-4e09-8e4d-20093ab576ba" />

**2. Ejecución del Worker:** Disparando la sincronización manual (o automática en background) para obtener y procesar datos.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/a6abf095-f5d4-48d4-bb2a-31ba3dfa58c7" />

**3. Bares Actualizado:** El sistema detecta los 8 bares del origen de datos, pero la IA descarta los duplicados, ingresando solo los bares únicos, a su vez la IA agrego descripciones de los bares. 
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/d55098fa-f000-47f5-b2fc-014b90c85258" />

**4. Procesamiento de IA en Acción:** Las tarjetas muestran cómo la IA asignó la categoría correcta y generó una descripción de marketing para cada lugar.
<img width="1402" height="670" alt="image" src="https://github.com/user-attachments/assets/487ba244-514a-4616-8361-423d6b31f2eb" />

**5. Dashboard:** Con el total de bares, cantidad de categorias, ultima sincronización y pie chart de los bares.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/640240d8-e594-4e72-831e-4f5263cc4b0d" />

**6. Historial de Logs:** Registro en la base de datos de la operación del Worker Service.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/fb57f2d6-3ccd-4da2-9c6a-da9774141aac" />

**7. Prevención de Duplicados Manuales:** El sistema bloquea el ingreso de un bar si detecta que el nombre es semánticamente idéntico a uno ya existente.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/32fd99b3-8ce5-4e7a-86aa-24ee503611a5" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/4e4d78f7-9c20-4149-9f74-dafbfda806b6" />


**8. Editar algun bar:** Se permite editar datos del bar.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/91f18cd5-7469-41ac-ad73-389076d02ec4" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/5edecf6e-e0e0-440f-830b-62ee51021d3d" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/6bc545cd-972e-406e-a996-931784a3f2c6" />


**9. Creación manual de un bar:** Dar de alta un bar manualmente como agregado del sistema.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/625c46f2-a20f-4cfb-9229-15941533b1df" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/962203c0-bbf8-4dbe-980c-cd98d1905071" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/0c5d712e-e8c8-489e-bcf5-438d8aa988c9" />


**10. Eliminar algun bar**: Dar de baja lógica a algun bar con modal de confirmación.
<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/d99925dc-e163-4f71-882e-8fa65cc1922f" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/6d5c9169-7088-4911-9521-c1b15ccd95c5" />

<img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/1ecbfd20-25ff-4fd7-a790-159e2f0f025b" />

---

## Demostración datos técnicos

**1. Mock de bares**: Bares con los que se probo el flujo, ingresando bares repetidos para probar que no se agregaban duplicados.
```json
[
  {
    "nombre": "Bar Irlanda Tucumán",
    "ubicacion": "Catamarca 380",
    "categoria": "Bar",
    "fuente": "MockData"
  },
  {
    "nombre": "Irlanda Bar",
    "ubicacion": "Catamarca 380",
    "categoria": "Pub",
    "fuente": "MockData"
  },
  {
    "nombre": "Peñón del Águila",
    "ubicacion": "San Lorenzo 400, Barrio Sur",
    "categoria": "Cervecería",
    "fuente": "MockData"
  },
  {
    "nombre": "Peñón del Águila Barrio Sur",
    "ubicacion": "San Lorenzo 400",
    "categoria": "Cervecería Artesanal",
    "fuente": "MockData"
  },
  {
    "nombre": "Porter Brew House",
    "ubicacion": "San Martín 780",
    "categoria": "Cervecería",
    "fuente": "MockData"
  },
  {
    "nombre": "Refugio Patagonia",
    "ubicacion": "Santa Fe 750",
    "categoria": "Restobar",
    "fuente": "MockData"
  },
  {
    "nombre": "Joao Bar",
    "ubicacion": "General Paz 516",
    "categoria": "Restobar",
    "fuente": "MockData"
  },
  {
    "nombre": "Mr. John's",
    "ubicacion": "25 de Mayo 410",
    "categoria": "Pub",
    "fuente": "MockData"
  }
]
```
**2. Sincronización lado backend:** Salida del backend con la sincronización realizada por primera vez, con los datos de los bares del mock y la base de datos vacia.
<img width="1919" height="1029" alt="image" src="https://github.com/user-attachments/assets/5f19b65e-ad7a-4e33-b03f-a610ec35cf74" />

**3. Base de datos:** Tabla de los bares con los mismos cargados y el bar eliminado en el flujo con su estado activo en falso.
<img width="1585" height="262" alt="image" src="https://github.com/user-attachments/assets/03dbd87a-377d-465a-b7af-c9704b4c1c4c" />

