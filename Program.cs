using ParcelApi.Services;
using ParcelApi.Data;
using ParcelApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IParcelService, EfParcelService>();
builder.Services.AddDbContext<ParcelDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ParcelDb"),
        o => o.UseNetTopologySuite()));
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:4200"];
builder.Services.AddCors(options =>
    options.AddPolicy("web", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcelDbContext>();
    if (!db.Parcels.Any())
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "parcels.json");
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var parcels = JsonSerializer.Deserialize<List<Parcel>>(json, options) ?? [];
        db.Parcels.AddRange(parcels);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("web");

app.UseAuthorization();

app.MapControllers();

app.Run();
