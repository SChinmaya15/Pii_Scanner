namespace PiiScanner.API.Entity
{
    public class FileRecord
    {
        public Guid Id { get; set; }
        public bool HasPii { get; set; }
        public string? Path { get; set; }
        public string? Source { get; set; } // AWS, Azure, etc
        public DateTime LastScanned { get; set; }
        public List<string> PiiTypes { get; set; } = [];
    }
}
