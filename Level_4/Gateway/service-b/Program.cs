var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Service B is running!");
app.MapGet("/health", () => "Healthy");
app.MapGet("/api/products", () => new { 
    message = "Products from Service B", 
    service = "B",
    count = 75,
    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});

app.Run("http://0.0.0.0:5002");
