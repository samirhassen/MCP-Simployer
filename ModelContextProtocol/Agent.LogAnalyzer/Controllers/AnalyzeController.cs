using Agent.LogAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.Text.RegularExpressions;

namespace Agent.LogAnalyzer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IOptions<AgentConfiguration> _agentConfig;
        private readonly IOptions<LogAnalysisConfiguration> _logAnalysisConfig;

        public AnalyzeController(
            IOptions<AgentConfiguration> agentConfig,
            IOptions<LogAnalysisConfiguration> logAnalysisConfig)
        {
            _agentConfig = agentConfig;
            _logAnalysisConfig = logAnalysisConfig;
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] AnalyzeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Logs))
                return BadRequest("Logs are required.");

            // Optionally call the Sanitizer agent to sanitize logs before analysis
            string sanitizerUrl = request.SanitizerUrl 
                ?? Environment.GetEnvironmentVariable("SANITIZER_URL")
                ?? _agentConfig.Value.SanitizerUrl;
            string sanitizedLogs = request.Logs;
            if (request.UseSanitizer)
            {
                using var client = new HttpClient();
                var sanitizeReq = new { Text = request.Logs };
                var response = await client.PostAsJsonAsync(sanitizerUrl, sanitizeReq);
                if (response.IsSuccessStatusCode)
                {
                    var sanitizeResult = await response.Content.ReadFromJsonAsync<SanitizeResponse>();
                    sanitizedLogs = sanitizeResult?.SanitizedText ?? request.Logs;
                }
            }

            // Simulate log analysis: count warnings and errors using configured patterns
            var patterns = _logAnalysisConfig.Value.Patterns;
            int warnings = Regex.Matches(sanitizedLogs, patterns["Warning"], RegexOptions.IgnoreCase).Count;
            int errors = Regex.Matches(sanitizedLogs, patterns["Error"], RegexOptions.IgnoreCase).Count;
            int critical = Regex.Matches(sanitizedLogs, patterns["Critical"], RegexOptions.IgnoreCase).Count;

            return Ok(new AnalyzeResponse
            {
                Warnings = warnings,
                Errors = errors,
                Critical = critical,
                Message = $"Analysis complete: {warnings} warnings, {errors} errors, {critical} critical."
            });
        }
    }
}
