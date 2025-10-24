# Project Chimera: Multi-Agent Orchestration System

## Overview
This project demonstrates a minimal Model Context Protocol (MCP)-style orchestration system, where a central orchestrator coordinates multiple autonomous agents. Each agent is a microservice with a specific capability, and the orchestrator plans, delegates, and aggregates their work.

## Architecture
- **Orchestrator**: Central service that receives requests, plans the workflow, delegates tasks, and aggregates results.
- **Agents**:
	- **Sanitizer**: Removes sensitive data from input.
	- **LogAnalyzer**: Analyzes logs for errors/warnings (calls Sanitizer if needed).
	- **ReportGenerator**: Produces executive summaries and visual assets from analysis.

Agents self-register with the orchestrator on startup, declaring their capabilities.

## Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- All projects restored and built successfully
- (Optional) [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore) for Swagger UI:
	```
	dotnet add package Swashbuckle.AspNetCore
	```
	(Run in the Orchestrator project directory)

### Running the System
1. **Start the Orchestrator**
	 - Run the Orchestrator project (e.g., `dotnet run --project Orchestrator`)
2. **Start Each Agent**
	 - Run each agent project in a separate terminal:
		 - `dotnet run --project Agent.Sanitizer`
		 - `dotnet run --project Agent.LogAnalyzer`
		 - `dotnet run --project Agent.ReportGenerator`
3. **Verify Agent Registration**
	 - Visit `https://localhost:<orchestrator-port>/register` (GET) to see registered agents.

### Testing the End-to-End Flow
1. **Test Individual Agents**
	 - Use Swagger UI or Postman to POST to each agent’s endpoint:
		 - Sanitizer: `/sanitize` with `{ "text": "my email is test@example.com" }`
		 - LogAnalyzer: `/analyze` with `{ "logs": "error: something failed", "useSanitizer": true }`
		 - ReportGenerator: `/generate-report` with `{ "warnings": 2, "errors": 1, "critical": 1 }`
2. **Test Orchestration**
	 - POST to the orchestrator’s `/plan-and-run` endpoint:
		 - Example body: `{ "rawLogs": "user john.doe@example.com had error: disk full" }`
	 - You should receive a combined output, e.g.:
		 ```
		 === Project Chimera Results ===
		 Sanitizer → user [REDACTED] had error: disk full
		 LogAnalyzer → Analysis complete: 0 warnings, 1 errors, 0 critical.
		 ReportGenerator → === Executive Summary ===
		 Warnings: 0
		 Errors: 1
		 Critical: 0
		 Visual Asset: [***CHART***]
		 ```

## Design Document
See `ModelContextProtocol/ProjectChimera_Design.md` for architecture diagram, orchestration flow, and scaling notes.

## Notes
- All services must be running for the orchestration to work.
- Ports may need to be adjusted in each agent’s `Program.cs` or launch settings.
- If you see Swagger errors, ensure Swashbuckle.AspNetCore is installed.


## More Example Requests & Responses

### 1. Sanitizer Agent
**Request:**
```
POST /sanitize
{
	"text": "Contact: alice.smith@company.com, phone: 555-123-4567"
}
```
**Response:**
```
{
	"sanitizedText": "Contact: [REDACTED], phone: [REDACTED]",
	"message": "Sanitization complete."
}
```

### 2. LogAnalyzer Agent
**Request:**
```
POST /analyze
{
	"logs": "warning: disk space low\nerror: failed to connect\ncritical: system halt",
	"useSanitizer": false
}
```
**Response:**
```
{
	"warnings": 1,
	"errors": 1,
	"critical": 1,
	"message": "Analysis complete: 1 warnings, 1 errors, 1 critical."
}
```

### 3. ReportGenerator Agent
**Request:**
```
POST /generate-report
{
	"warnings": 2,
	"errors": 1,
	"critical": 1
}
```
**Response:**
```
{
	"summary": "=== Executive Summary ===\nWarnings: 2\nErrors: 1\nCritical: 1\n\nVisual Asset: [***CHART***]",
	"visualAsset": "[***CHART***]",
	"message": "Report generated successfully."
}
```

### 4. Orchestrator End-to-End
**Request:**
```
POST /plan-and-run
{
	"rawLogs": "bob@domain.com warning: cpu high error: disk full"
}
```
**Response:**
```
{
	"output": "=== Project Chimera Results ===\nSanitizer → [REDACTED] warning: cpu high error: disk full\nLogAnalyzer → Analysis complete: 1 warnings, 1 errors, 0 critical.\nReportGenerator → === Executive Summary ===\nWarnings: 1\nErrors: 1\nCritical: 0\nVisual Asset: [***CHART***]",
	"visual": "[***CHART***]"
}
```

---
For questions or troubleshooting, please check the agent and orchestrator logs for errors.
