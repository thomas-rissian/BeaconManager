using BaliseManager.Api.Data;
using BaliseManager.Api.Repositories;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services;
using BaliseManager.Api.Services.Interfaces;
using BaliseManager.Api.WebSockets;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationSection allowedOriginsConfiguration =
    builder.Configuration.GetSection("WebSockets:AllowedOrigins");
string[] allowedOrigins = allowedOriginsConfiguration
    .GetChildren()
    .Select(section => section.Value)
    .OfType<string>()
    .ToArray();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBeaconRepository, BeaconRepository>();
builder.Services.AddScoped<IBeaconService, BeaconService>();
builder.Services.AddScoped<IBeaconEventRepository, BeaconEventRepository>();
builder.Services.AddScoped<IBeaconEventService, BeaconEventService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<BeaconEventWebSocketManager>();
builder.Services.AddSingleton<IBeaconEventNotifier>(
    serviceProvider => serviceProvider.GetRequiredService<
        BeaconEventWebSocketManager>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "api/docs";
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "BaliseManager API v1");
    });
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
WebSocketOptions webSocketOptions = new WebSocketOptions();
foreach (string allowedOrigin in allowedOrigins)
{
    webSocketOptions.AllowedOrigins.Add(allowedOrigin);
}

app.UseWebSockets(webSocketOptions);
app.UseMiddleware<BeaconEventWebSocketMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
