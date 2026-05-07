namespace BaresTucuman.API.Domain.Entities
{
    public class Bar
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;       // [cite: 29]
        public string Ubicacion { get; set; } = string.Empty;   // [cite: 30]
        public string Categoria { get; set; } = string.Empty;   // [cite: 31]
        public string Fuente { get; set; } = string.Empty;     // [cite: 32]
        public DateTime ScrapedAt { get; set; }                // [cite: 33]

        // Campos estratégicos para ganar puntos extra
        public bool IsActive { get; set; } = true;             // Para "Eliminarlos o desactivarlos" (Soft Delete) 
        public string AiDescription { get; set; } = string.Empty; // Para guardar la descripción generada por IA 
    }
}
