# Configuration Guide

This document explains how to configure the MCP-Simployer system using environment variables and configuration files.

## Environment Variables

The system supports the following environment variables for runtime configuration:

### Orchestrator Configuration
- `ORCHESTRATOR_URL`: URL for agent registration (default: http://localhost:5131/register)
- `ORCHESTRATOR_PORT`: Port for orchestrator service (default: 5131)

### Agent Endpoints
- `AGENT_ENDPOINT`: Base endpoint for the current agent
- `AGENT_NAME`: Name of the agent
- `AGENT_DESCRIPTION`: Description of the agent's capabilities

### Service URLs
- `SANITIZER_URL`: URL for sanitizer service (default: http://localhost:5197/sanitize)
- `ANALYZER_ENDPOINT`: URL for analyzer service (default: http://localhost:5074/analyze)
- `REPORT_ENDPOINT`: URL for report generator service (default: http://localhost:5192/GenerateReport/report)

### Log Analysis Patterns
- `LOG_PATTERN_WARNING`: Pattern for warning detection (default: warning)
- `LOG_PATTERN_ERROR`: Pattern for error detection (default: error)
- `LOG_PATTERN_CRITICAL`: Pattern for critical detection (default: critical)

### Tool Names
- `TOOL_SANITIZE`: Tool name for sanitization (default: sanitize)
- `TOOL_ANALYZE`: Tool name for analysis (default: analyze)
- `TOOL_GENERATE_REPORT`: Tool name for report generation (default: generate-report)

## Configuration Files

### appsettings.json
Each service has its own `appsettings.json` file with default configuration values:

- **Agent.LogAnalyzer**: Contains agent configuration and log analysis patterns
- **Agent.Sanitizer**: Contains agent configuration
- **Agent.ReportGenerator**: Contains agent configuration
- **Orchestrator**: Contains services and tool configuration

### appsettings.Development.json
Development-specific overrides for local development.

## Docker Configuration

Use the provided `docker-compose.yml` file to run the entire system with proper environment variable configuration.

### Running with Docker Compose
```bash
docker-compose up --build
```

### Running Individual Services
```bash
# Set environment variables
export ORCHESTRATOR_URL=http://localhost:5131/register
export AGENT_ENDPOINT=http://localhost:5074
export AGENT_NAME=LogAnalyzerAgent

# Run the service
dotnet run
```

## Configuration Priority

1. Environment variables (highest priority)
2. appsettings.Development.json (development only)
3. appsettings.json (default values)

## Examples

### Local Development
```bash
# Set environment variables for local development
export ORCHESTRATOR_URL=http://localhost:5131/register
export AGENT_ENDPOINT=http://localhost:5074
export AGENT_NAME=LogAnalyzerAgent
export SANITIZER_URL=http://localhost:5197/sanitize

# Run the log analyzer
cd ModelContextProtocol/Agent.LogAnalyzer
dotnet run
```

### Production Deployment
```bash
# Set production environment variables
export ASPNETCORE_ENVIRONMENT=Production
export ORCHESTRATOR_URL=http://orchestrator:80/register
export AGENT_ENDPOINT=http://log-analyzer:80
export AGENT_NAME=LogAnalyzerAgent

# Run the service
dotnet run
```

## Troubleshooting

### Common Issues

1. **Agent Registration Fails**: Check that `ORCHESTRATOR_URL` is correct and the orchestrator is running
2. **Service Communication Fails**: Verify that all service URLs are accessible
3. **Configuration Not Loading**: Ensure environment variables are set before starting the service

### Debugging Configuration

Add logging to see which configuration values are being used:

```csharp
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Using orchestrator URL: {OrchestratorUrl}", orchestratorUrl);
```
