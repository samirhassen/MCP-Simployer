using Agent.Sanitizer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Agent.Sanitizer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SanitizeController : ControllerBase
    {
        // POST /sanitize
        [HttpPost]
        public IActionResult Sanitize([FromBody] SanitizeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required.");

            // Simulate sanitization: replace anything that looks like an email or number with [REDACTED]
            string sanitized = Regex.Replace(request.Text, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", "[REDACTED]");
            sanitized = Regex.Replace(sanitized, @"\b\d{3,}\b", "[REDACTED]");

            return Ok(new SanitizeResponse
            {
                SanitizedText = sanitized,
                Message = "Sanitization complete."
            });
        }
    }



}
