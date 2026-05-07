using System.Text;
using System.Text.Json;
using BaresTucuman.API.Domain;
using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Enums;
using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaresTucuman.API.Services
{
    public class BarSyncService : IBarSyncService
    {
        private readonly IBarProvider _barProvider;
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;

        public BarSyncService(IBarProvider barProvider, AppDbContext context, HttpClient httpClient, IConfiguration config)
        {
            _barProvider = barProvider;
            _context = context;
            _httpClient = httpClient;
            _geminiApiKey = config["Gemini:ApiKey"] ?? throw new ArgumentNullException("Falta la API Key de Gemini");
        }

        public async Task<int> SyncBaresAsync()
        {
            var log = new SyncLog { IsSuccess = false }; 
            int baresNuevosAgregados = 0;

            try
            {
                var baresExternos = await _barProvider.GetBaresAsync();
                var baresEnDb = await _context.Bares.ToListAsync();


                foreach (var barExterno in baresExternos)
                {
                    bool existe = false;

                    try
                    {
                        existe = await EsDuplicadoConIA(barExterno.Nombre, barExterno.Ubicacion, baresEnDb);
                    }
                    catch (Exception)
                    {
                        existe = false;
                    }

                    if (!existe)
                    {
                        existe = baresEnDb.Any(b => NombresSonSimilares(b.Nombre, barExterno.Nombre));
                    }

                    if (!existe)
                    {
                        var descripcionAi = await GenerarDescripcionConIA(barExterno.Nombre, barExterno.Ubicacion);

                        var barNuevo = new Bar
                        {
                            Nombre = barExterno.Nombre,
                            Ubicacion = barExterno.Ubicacion,
                            Categoria = barExterno.Categoria,
                            CategoriaAMostrar = MapearCategoria(barExterno.Categoria),
                            Fuente = barExterno.Fuente,
                            AiDescription = descripcionAi,
                            ScrapedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        baresEnDb.Add(barNuevo);
                        _context.Bares.Add(barNuevo);
                        await _context.SaveChangesAsync();
                        baresNuevosAgregados++;

                        await Task.Delay(4000);
                    }
                    
                }

                log.IsSuccess = true;
                log.BarsAdded = baresNuevosAgregados;
            }
            catch (Exception ex)
            {
                log.ErrorMessage = ex.Message;
                throw; 
            }
            finally
            {
                _context.SyncLogs.Add(log);
                await _context.SaveChangesAsync();
            }

            return baresNuevosAgregados;
        }

        private async Task<string> GenerarDescripcionConIA(string nombre, string ubicacion)
        {
            try
            {
                var prompt = $"Sos un experto en turismo de Tucumán. Escribí una descripción atractiva de máximo 2 líneas para un bar llamado '{nombre}' ubicado en '{ubicacion}'. No uses formato markdown, solo texto plano.";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_geminiApiKey}";
                var response = await _httpClient.PostAsync(url, content);

                //if (!response.IsSuccessStatusCode) return "Descripción no disponible momentáneamente.";
                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    return $"Error API: {response.StatusCode} - {errorJson}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                var descripcion = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                return descripcion?.Trim() ?? "Sin descripción generada.";
            }
            catch (Exception)
            {
                return "Error de conexión con la IA.";
            }
        }

        private TipoBar MapearCategoria(string rawCategory)
        {
            var texto = rawCategory.ToLower();

            if (texto.Contains("cervece") || texto.Contains("brew")) return TipoBar.Cervecerias;
            if (texto.Contains("cafe") || texto.Contains("pastelería")) return TipoBar.Cafeterias;
            if (texto.Contains("resto") || texto.Contains("parrilla") || texto.Contains("comida")) return TipoBar.Restobares;
            if (texto.Contains("pub") || texto.Contains("disco") || texto.Contains("boliche")) return TipoBar.Pubs;

            return TipoBar.BaresClasicos;
        }

        private async Task<bool> EsDuplicadoConIA(string nombreNuevo, string ubicacionNueva, List<Bar> baresExistentes)
        {
            if (!baresExistentes.Any()) return false; 

            try
            {
                var listaExistentesStr = string.Join("\n", baresExistentes.Select(b => $"- {b.Nombre} (Ubicación: {b.Ubicacion})"));

                var prompt = $"Sos un analista de datos. Tengo un nuevo bar llamado '{nombreNuevo}' ubicado en '{ubicacionNueva}'. " +
                             $"¿Es este bar el mismo establecimiento que alguno de esta lista de bares existentes?\n{listaExistentesStr}\n" +
                             $"Considerá que los nombres pueden estar escritos en distinto orden, tener palabras extra o que la ubicación sea aproximada. " +
                             $"Respondé ESTRICTAMENTE con la palabra 'SI' o 'NO', sin puntos ni explicaciones.";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiApiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode) return false;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);

                var respuestaIA = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString()?.Trim().ToUpper();

                return respuestaIA == "SI";
            }
            catch
            {
                return false;
            }
        }

        private bool NombresSonSimilares(string nombre1, string nombre2)
        {
            var palabras1 = nombre1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var palabras2 = nombre2.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var ignorar = new HashSet<string> { "el", "la", "los", "las", "bar", "resto", "restobar", "tucuman", "tucumán", "pub" };

            var claves1 = palabras1.Where(p => !ignorar.Contains(p)).ToList();
            var claves2 = palabras2.Where(p => !ignorar.Contains(p)).ToList();

            if (!claves1.Any() || !claves2.Any()) return nombre1.ToLower() == nombre2.ToLower();

            var interseccion = claves1.Intersect(claves2).Count();

            return interseccion >= 1;
        }
    }
}