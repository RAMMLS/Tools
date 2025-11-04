using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => "Healthy");
app.MapGet("/api/products", () => new { message = "Products from Service B", count = 75 });
app.MapGet("/api/products/{id}", (int id) => new { id, name = $"Product {id}", service = "B" });
app.MapPost("/api/products", () => new { message = "Product created in Service B" });

app.Run("http://0.0.0.0:5003");
