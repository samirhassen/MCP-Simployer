using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Orchestrator.Services
{
    public class AgentInfo
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public List<string> Tools { get; set; }
        public string Description { get; set; }
    }

    public class AgentRegistry
    {
        private readonly ConcurrentDictionary<string, AgentInfo> _agents = new();

        public void Register(AgentInfo agent)
        {
            if (!string.IsNullOrWhiteSpace(agent.Name) && !string.IsNullOrWhiteSpace(agent.Endpoint))
            {
                _agents[agent.Name] = agent;
            }
        }

        public List<AgentInfo> GetAll() => new List<AgentInfo>(_agents.Values);
    }
}
