using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Service A is running!");
app.MapGet("/health", () => "Healthy");
app.MapGet("/api/users", () => new { 
    message = "Users from Service A", 
    service = "A",
    count = 150,
    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});
app.MapGet("/api/users/{id}", (int id) => new { 
    id, 
    name = $"User {id}", 
    service = "A",
    email = $"user{id}@service-a.com"
});

app.Run("http://0.0.0.0:5001");
