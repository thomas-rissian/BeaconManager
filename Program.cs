using BaliseManager.Api.Data;
using BaliseManager.Api.Repositories;
using BaliseManager.Api.Repositories.Interfaces;
using BaliseManager.Api.Services;
using BaliseManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
