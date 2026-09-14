using RoadSense.Application.Services;
var builder = WebApplication.CreateBuilder(args);

// Controllers: business logic lives in Application/Domain, controllers stay thin.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSingleton<IHazardService, HazardService>();
// OpenAPI document generation (built into the ASP.NET Core templates on .NET 10).
builder.Services.AddOpenApi();

// Application / Infrastructure service registrations will be added here
// starting Phase 2, via extension methods such as:
//   builder.Services.AddApplicationServices();
//   builder.Services.AddInfrastructureServices(builder.Configuration);
// Keeping Program.cs free of ad-hoc registrations now avoids it becoming
// a dumping ground later.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
