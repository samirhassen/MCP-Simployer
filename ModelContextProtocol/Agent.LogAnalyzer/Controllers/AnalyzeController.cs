using Agent.LogAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Agent.LogAnalyzer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyzeController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] AnalyzeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Logs))
                return BadRequest("Logs are required.");

            // Optionally call the Sanitizer agent to sanitize logs before analysis
            string sanitizerUrl = request.SanitizerUrl ?? "http://localhost:5197/sanitize";
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

            // Simulate log analysis: count warnings and errors
            int warnings = Regex.Matches(sanitizedLogs, "warning", RegexOptions.IgnoreCase).Count;
            int errors = Regex.Matches(sanitizedLogs, "error", RegexOptions.IgnoreCase).Count;
            int critical = Regex.Matches(sanitizedLogs, "critical", RegexOptions.IgnoreCase).Count;

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
