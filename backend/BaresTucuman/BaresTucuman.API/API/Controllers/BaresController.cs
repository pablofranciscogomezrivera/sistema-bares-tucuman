using BaresTucuman.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaresTucuman.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaresController : ControllerBase
    {
        private readonly IBarProvider _barProvider;

        // Inyección de dependencias por constructor
        public BaresController(IBarProvider barProvider)
        {
            _barProvider = barProvider;
        }

        [HttpGet("mock")]
        public async Task<IActionResult> GetMockPlaces()
        {
            // Llamamos a nuestro proveedor (que por ahora es el Mock leyendo el JSON)
            var places = await _barProvider.GetPlacesAsync();

            if (places == null || !places.Any())
            {
                return NotFound("No se encontraron bares. Revisá si configuraste el JSON para copiarse al compilar.");
            }

            // Devolvemos un HTTP 200 OK con la lista de bares
            return Ok(places);
        }
    }
}
