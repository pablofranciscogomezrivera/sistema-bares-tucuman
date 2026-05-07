namespace BaresTucuman.API.Domain.Entities
{
    public class SyncLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int BarsAdded { get; set; }
        public string? ErrorMessage { get; set; } 
        public bool IsSuccess { get; set; }
    }
}