using Microsoft.EntityFrameworkCore;
using RoadSense.Api.Hubs;
using RoadSense.Application.Repositories;
using RoadSense.Application.Services;
using RoadSense.Infrastructure.Data;
using RoadSense.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<IHazardService, HazardService>();
builder.Services.AddScoped<IHazardRepository, HazardRepository>();

builder.Services.AddDbContext<RoadSenseDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("RoadSenseDb"),
        npgsqlOptions =>
        {
            npgsqlOptions.UseNetTopologySuite();
        }));

// SignalR for real-time hazard communication.
builder.Services.AddSignalR();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Real-time hazard communication endpoint.
app.MapHub<HazardHub>("/hubs/hazards");

app.Run();

public partial class Program
{
}