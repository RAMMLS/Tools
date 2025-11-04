var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "API Gateway is running!");
app.MapGet("/health", () => "Healthy");
app.MapGet("/api/users", () => new { 
    message = "Users from Gateway", 
    gateway = "Docker",
    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});
app.MapGet("/api/products", () => new { 
    message = "Products from Gateway", 
    gateway = "Docker",
    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});

app.Run("http://0.0.0.0:5000");
