

using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Agent registration logic
        var orchestratorUrl = builder.Configuration["OrchestratorUrl"] ?? "http://localhost:5131/register";
        var agentInfo = new
        {
            Name = "SanitizerAgent",
            Endpoint = "http://localhost:5197", // Update as needed
            Tools = new[] { "sanitize" },
            Description = "Sanitizes sensitive data from input."
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
                    Console.WriteLine("Sanitizer agent registered successfully.");
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
