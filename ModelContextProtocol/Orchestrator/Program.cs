using Microsoft.Extensions.Options;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Orchestrator.Services.AgentRegistry>();
builder.Services.AddHttpClient();

// Configure options
builder.Services.Configure<ServicesConfiguration>(
    builder.Configuration.GetSection("ServicesConfiguration"));
builder.Services.Configure<ToolConfiguration>(
    builder.Configuration.GetSection("ToolConfiguration"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
