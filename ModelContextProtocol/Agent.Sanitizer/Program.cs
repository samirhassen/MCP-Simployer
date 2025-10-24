

using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Shared.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure services
        builder.Services.AddControllers();
        
        // Configure options
        builder.Services.Configure<AgentConfiguration>(
            builder.Configuration.GetSection("AgentConfiguration"));
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Agent registration logic using configuration
        var agentConfig = app.Services.GetRequiredService<IOptions<AgentConfiguration>>().Value;
        var orchestratorUrl = Environment.GetEnvironmentVariable("ORCHESTRATOR_URL") 
            ?? agentConfig.OrchestratorUrl;
        var agentEndpoint = Environment.GetEnvironmentVariable("AGENT_ENDPOINT") 
            ?? agentConfig.Endpoint;
        var agentName = Environment.GetEnvironmentVariable("AGENT_NAME") 
            ?? agentConfig.Name;
        var agentDescription = Environment.GetEnvironmentVariable("AGENT_DESCRIPTION") 
            ?? agentConfig.Description;

        var agentInfo = new
        {
            Name = agentName,
            Endpoint = agentEndpoint,
            Tools = agentConfig.Tools,
            Description = agentDescription
        };

        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.PostAsJsonAsync(orchestratorUrl, agentInfo);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to register agent: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine($"{agentName} registered successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering agent: {ex.Message}");
            }
        }

        await app.RunAsync();
    }
}
