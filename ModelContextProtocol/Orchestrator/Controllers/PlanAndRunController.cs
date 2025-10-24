using Microsoft.AspNetCore.Mvc;
using Orchestrator.Models;
using Orchestrator.Services;
using System.Text;

namespace Orchestrator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanAndRunController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AgentRegistry _agentRegistry;

        public PlanAndRunController(IHttpClientFactory httpClientFactory, AgentRegistry agentRegistry)
        {
            _httpClientFactory = httpClientFactory;
            _agentRegistry = agentRegistry;
        }

        [HttpPost]
        public async Task<IActionResult> PlanAndRun([FromBody] PlanAndRunRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RawLogs))
                return BadRequest("RawLogs is required.");

            // 1. Get registered agents
            var agents = _agentRegistry.GetAll();
            var sanitizer = agents.Find(a => a.Tools != null && a.Tools.Contains("sanitize"));
            var analyzer = agents.Find(a => a.Tools != null && a.Tools.Contains("analyze"));
            var reporter = agents.Find(a => a.Tools != null && a.Tools.Contains("generate-report"));
            if (sanitizer == null || analyzer == null || reporter == null)
                return StatusCode(500, "One or more required agents are not registered.");

            var client = _httpClientFactory.CreateClient();

            // 2. Call Sanitizer
            string sanitizedLogs = request.RawLogs;
            var sanitizeReq = new { Text = request.RawLogs };
            var sanitizeResp = await client.PostAsJsonAsync($"{sanitizer.Endpoint}/sanitize", sanitizeReq);
            if (sanitizeResp.IsSuccessStatusCode)
            {
                var sanitizeResult = await sanitizeResp.Content.ReadFromJsonAsync<SanitizeResponse>();
                sanitizedLogs = sanitizeResult?.SanitizedText ?? request.RawLogs;
            }

            // 3. Call LogAnalyzer (which may call Sanitizer again)
            AnalyzeResponse? analysis = null;
            var analyzeReq = new { Logs = sanitizedLogs, UseSanitizer = false };
            var analyzeResp = await client.PostAsJsonAsync($"{analyzer.Endpoint}/analyze", analyzeReq);
            if (analyzeResp.IsSuccessStatusCode)
            {
                analysis = await analyzeResp.Content.ReadFromJsonAsync<AnalyzeResponse>();
            }
            if (analysis == null)
                return StatusCode(500, "Log analysis failed.");

            // 4. Call ReportGenerator
            GenerateReportResponse? report = null;
            var reportReq = new { analysis.Warnings, analysis.Errors, analysis.Critical };
            var reportResp = await client.PostAsJsonAsync($"{reporter.Endpoint}/GenerateReport/report", reportReq);
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
