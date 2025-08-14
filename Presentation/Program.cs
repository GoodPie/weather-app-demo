using System.Threading.RateLimiting;
using BLL.Services;
using BLL.Services.Contracts;
using DAL;
using DAL.Repository;
using DAL.Repository.Contracts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

// Add CORS policy to allow requests from the Vite development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Vite dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Rate limiting middleware
// This will prevent key abuse (on top of API key limiting from provider)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", rateLimitOptions =>
    {
        rateLimitOptions.Window = TimeSpan.FromSeconds(10);
        rateLimitOptions.PermitLimit = 10; // Allow 5 requests per 10 seconds
        rateLimitOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        rateLimitOptions.QueueLimit = 2; // Small queue to prevent abuse
        rateLimitOptions.AutoReplenishment = true;
    });
});

// Use the DAL-configured database path for consistency
builder.Services.AddDbContext<WeatherDbContext>(options =>
{
    options.UseSqlite("Data Source=../DAL/Database/WeatherApp.db;Cache=Shared;");
});

// Inject repository layer
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

// Inject service layer
builder.Services.AddScoped<ILocationService, LocationService>();

// Add HttpClient and Geocoding service
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGeocodingService, GoogleGeocodingService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// In memory caching - We are using Sqlite but we can use in-memory caching for quick lookups
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("DevCors");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();