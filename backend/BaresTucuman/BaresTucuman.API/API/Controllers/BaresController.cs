using BaresTucuman.API.Application.DTOs;
using BaresTucuman.API.Application.Helpers;
using BaresTucuman.API.Domain.Entities;
using BaresTucuman.API.Domain.Interfaces;
using BaresTucuman.API.Infraestructure.Data;
using BaresTucuman.API.Services;
using FluentValidation;
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
        private readonly IBarSyncService _syncService;

        public BaresController(IBarProvider barProvider, AppDbContext context, IBarSyncService syncService)
        {
            _barProvider = barProvider;
            _context = context;
            _syncService = syncService;
        }

        [HttpGet("mock")]
        public async Task<IActionResult> GetMockBares()
        {
            var places = await _barProvider.GetBaresAsync();
            if (places == null || !places.Any()) return NotFound("No se encontraron bares.");
            return Ok(places);
        }

        [HttpGet]
        public async Task<IActionResult> GetBares([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Bares.Where(b => b.IsActive).OrderByDescending(b => b.ScrapedAt);

            var total = await query.CountAsync();
            var bares = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize),
                data = bares
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBar(int id)
        {
            var bar = await _context.Bares.FindAsync(id);
            if (bar == null || !bar.IsActive) return NotFound($"No se encontró el bar con ID {id}");
            return Ok(bar);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBar(
            int id,
            [FromBody] ActualizarBarDto dto,
            [FromServices] IValidator<ActualizarBarDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => new { Campo = e.PropertyName, Error = e.ErrorMessage });
                return BadRequest(errores);
            }

            var barExistente = await _context.Bares.FindAsync(id);
            if (barExistente == null || !barExistente.IsActive)
                return NotFound($"No se encontró el bar activo con ID {id}");

            barExistente.Nombre = dto.Nombre;
            barExistente.Ubicacion = dto.Ubicacion;
            barExistente.Categoria = dto.Categoria;
            barExistente.CategoriaAMostrar = dto.CategoriaAMostrar;
            barExistente.AiDescription = dto.AiDescription;

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

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _context.Bares
                .Where(b => b.IsActive)
                .GroupBy(b => b.CategoriaAMostrar)
                .Select(g => new
                {
                    Categoria = g.Key.ToString(), 
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();

            return Ok(stats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBar(
            [FromBody] CrearBarDto dto,
            [FromServices] IValidator<CrearBarDto> validator)
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errores = validationResult.Errors.Select(e => new { Campo = e.PropertyName, Error = e.ErrorMessage });
                return BadRequest(errores);
            }

            var baresExistentes = await _context.Bares.Where(b => b.IsActive).ToListAsync();
            bool esDuplicado = baresExistentes.Any(b => BarHelper.NombresSonSimilares(b.Nombre, dto.Nombre));

            if (esDuplicado)
            {
                return BadRequest("El bar que intentas registrar ya existe o tiene un nombre muy similar a uno activo.");
            }

            var nuevoBar = new Bar
            {
                Nombre = dto.Nombre,
                Ubicacion = dto.Ubicacion,
                Categoria = dto.Categoria,
                CategoriaAMostrar = dto.CategoriaAMostrar,
                IsActive = true,
                ScrapedAt = DateTime.UtcNow,
                Fuente = "Carga Manual",
                AiDescription = "Descripción pendiente de generación."
            };

            _context.Bares.Add(nuevoBar);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBar), new { id = nuevoBar.Id }, nuevoBar);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> TriggerSync()
        {
            int agregados = await _syncService.SyncBaresAsync();
            return Ok(new { message = "Sincronización completada.", baresAgregados = agregados });
        }

        [HttpGet("sync/logs")]
        public async Task<IActionResult> GetSyncLogs()
        {
            var logs = await _context.SyncLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(20)
                .ToListAsync();
            return Ok(logs);
        }

    }
}