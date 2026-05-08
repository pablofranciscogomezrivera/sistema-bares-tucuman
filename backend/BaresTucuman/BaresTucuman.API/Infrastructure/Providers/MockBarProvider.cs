using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using System.Text.Json;

namespace BaresTucuman.API.Infraestructure.Providers
{   
    public class MockBarProvider : IBarProvider
    {
        private readonly IWebHostEnvironment _env;
        public MockBarProvider(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<List<Bar>> GetBaresAsync()
        {
            var jsonPath = Path.Combine(_env.ContentRootPath, "Infrastructure", "Data", "bares_tucuman.json");
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine(_env.ContentRootPath, "Infraestructure", "Data", "bares_tucuman.json");
            }
            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"ERROR CRÍTICO: No se encontró el archivo JSON en: {jsonPath}");
            }

            var jsonString = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var bares = JsonSerializer.Deserialize<List<Bar>>(jsonString, options);

            return bares ?? new List<Bar>();
        }
    }
}

