using BaresTucuman.API.Domain.Enums;

namespace BaresTucuman.API.Application.Helpers
{
    public static class BarHelper
    {
        public static TipoBar MapearCategoria(string rawCategory)
        {
            if (string.IsNullOrEmpty(rawCategory)) return TipoBar.BaresClasicos;

            var texto = rawCategory.ToLower();
            if (texto.Contains("cervece") || texto.Contains("brew")) return TipoBar.Cervecerias;
            if (texto.Contains("cafe") || texto.Contains("pastelería")) return TipoBar.Cafeterias;
            if (texto.Contains("resto") || texto.Contains("parrilla") || texto.Contains("comida")) return TipoBar.Restobares;
            if (texto.Contains("pub") || texto.Contains("disco") || texto.Contains("boliche")) return TipoBar.Pubs;

            return TipoBar.BaresClasicos;
        }

        public static bool NombresSonSimilares(string nombre1, string nombre2)
        {
            if (string.IsNullOrEmpty(nombre1) || string.IsNullOrEmpty(nombre2)) return false;

            var palabras1 = nombre1.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var palabras2 = nombre2.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var ignorar = new HashSet<string> { "el", "la", "los", "las", "bar", "resto", "restobar", "tucuman", "tucumán", "pub" };

            var claves1 = palabras1.Where(p => !ignorar.Contains(p)).ToList();
            var claves2 = palabras2.Where(p => !ignorar.Contains(p)).ToList();

            if (!claves1.Any() || !claves2.Any()) return nombre1.ToLower() == nombre2.ToLower();

            return claves1.Intersect(claves2).Any();
        }
    }
}