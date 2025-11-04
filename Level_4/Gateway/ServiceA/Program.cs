using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => "Healthy");
app.MapGet("/api/users", () => new { message = "Users from Service A", count = 150 });
app.MapGet("/api/users/{id}", (int id) => new { id, name = $"User {id}", service = "A" });
app.MapPost("/api/users", () => new { message = "User created in Service A" });

app.Run("http://0.0.0.0:5001");
