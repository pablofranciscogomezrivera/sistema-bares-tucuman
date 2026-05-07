using BaresTucuman.API.Domain.Enums;

namespace BaresTucuman.API.Application.DTOs
{
    public class ActualizarBarDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public TipoBar CategoriaAMostrar { get; set; }
        public string AiDescription { get; set; } = string.Empty; 
    }
}