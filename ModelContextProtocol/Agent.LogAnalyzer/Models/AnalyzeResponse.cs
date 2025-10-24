namespace Agent.LogAnalyzer.Models
{
    public class AnalyzeResponse
    {
        public int Warnings { get; set; }
        public int Errors { get; set; }
        public int Critical { get; set; }
        public string Message { get; set; }
    }
}
