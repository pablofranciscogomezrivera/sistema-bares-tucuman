using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using System.Text.Json;

namespace BaresTucuman.API.Infraestructure.Providers
{   
    public class MockBarProvider : IBarProvider
    {
        public async Task<List<Bar>> GetBaresAsync()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Infraestructure", "Data", "bares_tucuman.json");

            if (!File.Exists(jsonPath))
                return new List<Bar>(); 

            var jsonString = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var bares = JsonSerializer.Deserialize<List<Bar>>(jsonString, options);

            return bares ?? new List<Bar>();
        }
    }
}

