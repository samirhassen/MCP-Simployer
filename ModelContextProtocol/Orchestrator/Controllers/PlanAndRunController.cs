using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orchestrator.Models;
using Orchestrator.Services;
using Shared.Configuration;
using System.Text;

namespace Orchestrator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanAndRunController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AgentRegistry _agentRegistry;
        private readonly IOptions<ServicesConfiguration> _servicesConfig;
        private readonly IOptions<ToolConfiguration> _toolConfig;

        public PlanAndRunController(
            IHttpClientFactory httpClientFactory, 
            AgentRegistry agentRegistry,
            IOptions<ServicesConfiguration> servicesConfig,
            IOptions<ToolConfiguration> toolConfig)
        {
            _httpClientFactory = httpClientFactory;
            _agentRegistry = agentRegistry;
            _servicesConfig = servicesConfig;
            _toolConfig = toolConfig;
        }

        [HttpPost]
        public async Task<IActionResult> PlanAndRun([FromBody] PlanAndRunRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RawLogs))
                return BadRequest("RawLogs is required.");

            // 1. Get registered agents using configured tool names
            var agents = _agentRegistry.GetAll();
            var toolConfig = _toolConfig.Value;
            var sanitizer = agents.Find(a => a.Tools != null && a.Tools.Contains(toolConfig.Sanitize));
            var analyzer = agents.Find(a => a.Tools != null && a.Tools.Contains(toolConfig.Analyze));
            var reporter = agents.Find(a => a.Tools != null && a.Tools.Contains(toolConfig.GenerateReport));
            if (sanitizer == null || analyzer == null || reporter == null)
                return StatusCode(500, "One or more required agents are not registered.");

            var client = _httpClientFactory.CreateClient();

            // 2. Call Sanitizer
            string sanitizedLogs = request.RawLogs;
            var sanitizeReq = new { Text = request.RawLogs };
            var sanitizeEndpoint = Environment.GetEnvironmentVariable("SANITIZER_ENDPOINT") 
                ?? $"{sanitizer.Endpoint}/sanitize";
            var sanitizeResp = await client.PostAsJsonAsync(sanitizeEndpoint, sanitizeReq);
            if (sanitizeResp.IsSuccessStatusCode)
            {
                var sanitizeResult = await sanitizeResp.Content.ReadFromJsonAsync<SanitizeResponse>();
                sanitizedLogs = sanitizeResult?.SanitizedText ?? request.RawLogs;
            }

            // 3. Call LogAnalyzer (which may call Sanitizer again)
            AnalyzeResponse? analysis = null;
            var analyzeReq = new { Logs = sanitizedLogs, UseSanitizer = false };
            var analyzeEndpoint = Environment.GetEnvironmentVariable("ANALYZER_ENDPOINT") 
                ?? $"{analyzer.Endpoint}/analyze";
            var analyzeResp = await client.PostAsJsonAsync(analyzeEndpoint, analyzeReq);
            if (analyzeResp.IsSuccessStatusCode)
            {
                analysis = await analyzeResp.Content.ReadFromJsonAsync<AnalyzeResponse>();
            }
            if (analysis == null)
                return StatusCode(500, "Log analysis failed.");

            // 4. Call ReportGenerator
            GenerateReportResponse? report = null;
            var reportReq = new { analysis.Warnings, analysis.Errors, analysis.Critical };
            var reportEndpoint = Environment.GetEnvironmentVariable("REPORT_ENDPOINT") 
                ?? $"{reporter.Endpoint}/GenerateReport/report";
            var reportResp = await client.PostAsJsonAsync(reportEndpoint, reportReq);
            if (reportResp.IsSuccessStatusCode)
            {
                report = await reportResp.Content.ReadFromJsonAsync<GenerateReportResponse>();
            }
            if (report == null)
                return StatusCode(500, "Report generation failed.");

            // 5. Aggregate and return results
            var sb = new StringBuilder();
            sb.AppendLine("=== Project Chimera Results ===");
            sb.AppendLine($"Sanitizer → {sanitizedLogs}");
            sb.AppendLine($"LogAnalyzer → {analysis.Message}");
            sb.AppendLine($"ReportGenerator → {report.Summary}");

            return Ok(new { Output = sb.ToString(), Visual = report.VisualAsset });
        }
    }

    public class PlanAndRunRequest
    {
        public string? RawLogs { get; set; }
    }
}
