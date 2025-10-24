namespace Agent.LogAnalyzer.Models
{
    public class AnalyzeRequest
    {
        public string Logs { get; set; }
        public bool UseSanitizer { get; set; } = true;
        public string? SanitizerUrl { get; set; } // Optional override
    }
}
