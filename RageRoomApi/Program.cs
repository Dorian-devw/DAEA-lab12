using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RageRoomApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Controladores y prevención de ciclos JSON sin DTOs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Configuración de DbContext con SQL Server
builder.Services.AddDbContext<RageRoomDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// 3. Configuración de CORS con orígenes concretos (Requisito: no usar AllowAnyOrigin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://rageroom-app.vercel.app",
                "https://rageroom.monsterasp.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 4. OpenAPI / Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. SwaggerUI visible tanto en desarrollo como en la API publicada (Requisito obligatorio)
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Rage Room API v1");
    // Permite acceder a SwaggerUI tanto desde / como desde /swagger
    options.RoutePrefix = "swagger";
});

// Redirigir la raíz / a /swagger para comodidad del evaluador en la nube
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
