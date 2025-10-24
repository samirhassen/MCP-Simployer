using Agent.ReportGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Agent.ReportGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenerateReportController : ControllerBase
    {
        // POST /generate-report
        [HttpPost("report")]
        public IActionResult GenerateReport([FromBody] GenerateReportRequest request)
        {
            if (request == null)
                return BadRequest("Request is required.");

            // Simulate summary
            var sb = new StringBuilder();
            sb.AppendLine("=== Executive Summary ===");
            sb.AppendLine($"Warnings: {request.Warnings}");
            sb.AppendLine($"Errors: {request.Errors}");
            sb.AppendLine($"Critical: {request.Critical}");
            sb.AppendLine();
            sb.AppendLine("Visual Asset: [***CHART***]");

            return Ok(new GenerateReportResponse
            {
                Summary = sb.ToString(),
                VisualAsset = "[***CHART***]",
                Message = "Report generated successfully."
            });
        }
    }

}
