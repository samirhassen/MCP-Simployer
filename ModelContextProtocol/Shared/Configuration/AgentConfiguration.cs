namespace Shared.Configuration
{
    public class AgentConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] Tools { get; set; } = Array.Empty<string>();
        public string OrchestratorUrl { get; set; } = string.Empty;
        public string SanitizerUrl { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
    }

    public class LogAnalysisConfiguration
    {
        public Dictionary<string, string> Patterns { get; set; } = new()
        {
            { "Warning", "warning" },
            { "Error", "error" },
            { "Critical", "critical" }
        };
    }

    public class ServicesConfiguration
    {
        public string Orchestrator { get; set; } = string.Empty;
        public string Sanitizer { get; set; } = string.Empty;
        public string LogAnalyzer { get; set; } = string.Empty;
        public string ReportGenerator { get; set; } = string.Empty;
    }

    public class ToolConfiguration
    {
        public string Sanitize { get; set; } = "sanitize";
        public string Analyze { get; set; } = "analyze";
        public string GenerateReport { get; set; } = "generate-report";
    }
}
