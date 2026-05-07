using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using System.Text.Json;

namespace BaresTucuman.API.Infraestructure.Providers
{   
    public class MockBarProvider : IBarProvider
    {
        public async Task<List<Bar>> GetPlacesAsync()
        {
            // Lee el archivo que configuraste para que se copie al compilar
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Data", "bares_tucuman.json");

            if (!File.Exists(jsonPath))
                return new List<Bar>(); // Si no lo encuentra, devuelve lista vacía para no romper

            var jsonString = await File.ReadAllTextAsync(jsonPath);

            // Opciones para que no le importen las mayúsculas/minúsculas en los nombres de las propiedades
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var places = JsonSerializer.Deserialize<List<Bar>>(jsonString, options);

            return places ?? new List<Bar>();
        }
    }
}

