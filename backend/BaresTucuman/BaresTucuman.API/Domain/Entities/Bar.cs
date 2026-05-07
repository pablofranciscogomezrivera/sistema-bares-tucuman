using BaresTucuman.API.Domain.Enums;

namespace BaresTucuman.API.Domain.Entities
{
    public class Bar
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;       
        public string Ubicacion { get; set; } = string.Empty;   
        public string Categoria { get; set; } = string.Empty;
        public TipoBar CategoriaAMostrar { get; set; }
        public string Fuente { get; set; } = string.Empty;     
        public DateTime ScrapedAt { get; set; }               

        public bool IsActive { get; set; } = true;             
        public string AiDescription { get; set; } = string.Empty; 
    }
}
