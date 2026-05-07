using System.Text;
using System.Text.Json;
using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaresTucuman.API.Services
{
    public class BarSyncService
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
            var baresExternos = await _barProvider.GetBaresAsync();
            int baresNuevosAgregados = 0;

            foreach (var barExterno in baresExternos)
            {
                var existe = await _context.Bares
                    .AnyAsync(b => b.Nombre.ToLower() == barExterno.Nombre.ToLower() ||
                                   b.Nombre.ToLower().Contains(barExterno.Nombre.ToLower()) ||
                                   barExterno.Nombre.ToLower().Contains(b.Nombre.ToLower()));

                if (!existe)
                {
                    barExterno.AiDescription = await GenerarDescripcionConIA(barExterno.Nombre, barExterno.Ubicacion);
                    barExterno.ScrapedAt = DateTime.UtcNow;

                    _context.Bares.Add(barExterno);

                    await _context.SaveChangesAsync();

                    baresNuevosAgregados++;
                }
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

                if (!response.IsSuccessStatusCode) return "Descripción no disponible momentáneamente.";

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
    }
}