namespace Agent.ReportGenerator.Models
{
    public class GenerateReportRequest
    {
        public int Warnings { get; set; }
        public int Errors { get; set; }
        public int Critical { get; set; }
    }
}
