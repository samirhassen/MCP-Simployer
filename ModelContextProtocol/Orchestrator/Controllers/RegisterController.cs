using Microsoft.AspNetCore.Mvc;
using Orchestrator.Services;

namespace Orchestrator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AgentRegistry _agentRegistry;

        public RegisterController(AgentRegistry agentRegistry)
        {
            _agentRegistry = agentRegistry;
        }

        [HttpPost]
        public IActionResult RegisterAgent([FromBody] AgentInfo agent)
        {
            if (string.IsNullOrWhiteSpace(agent.Name) || string.IsNullOrWhiteSpace(agent.Endpoint))
                return BadRequest("Agent name and endpoint are required.");

            // Convert Tools to List if needed
            if (agent.Tools == null)
                agent.Tools = new List<string>();
            _agentRegistry.Register(agent);
            return Ok(new { message = $"Agent '{agent.Name}' registered successfully." });
        }

        [HttpGet]
        public IActionResult GetAgents()
        {
            return Ok(_agentRegistry.GetAll());
        }
    }
}
