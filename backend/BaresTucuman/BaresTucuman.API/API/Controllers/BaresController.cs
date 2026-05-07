using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaresTucuman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaresController : ControllerBase
    {
        private readonly IBarProvider _barProvider;
        private readonly AppDbContext _context; 

        public BaresController(IBarProvider barProvider, AppDbContext context)
        {
            _barProvider = barProvider;
            _context = context;
        }

        [HttpGet("mock")]
        public async Task<IActionResult> GetMockBares()
        {
            var places = await _barProvider.GetBaresAsync();
            if (places == null || !places.Any()) return NotFound("No se encontraron bares.");
            return Ok(places);
        }

        [HttpGet]
        public async Task<IActionResult> GetBares()
        {
            var bares = await _context.Bares
                                      .Where(b => b.IsActive)
                                      .OrderByDescending(b => b.ScrapedAt)
                                      .ToListAsync();
            return Ok(bares);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBar(int id)
        {
            var bar = await _context.Bares.FindAsync(id);
            if (bar == null || !bar.IsActive) return NotFound($"No se encontró el bar con ID {id}");
            return Ok(bar);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBar(int id, [FromBody] Bar barActualizado)
        {
            if (id != barActualizado.Id) return BadRequest("El ID de la URL no coincide con el cuerpo de la petición.");

            var barExistente = await _context.Bares.FindAsync(id);
            if (barExistente == null || !barExistente.IsActive) return NotFound();

            barExistente.Nombre = barActualizado.Nombre;
            barExistente.Ubicacion = barActualizado.Ubicacion;
            barExistente.Categoria = barActualizado.Categoria;
            barExistente.AiDescription = barActualizado.AiDescription;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBar(int id)
        {
            var bar = await _context.Bares.FindAsync(id);
            if (bar == null || !bar.IsActive) return NotFound();

            bar.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}