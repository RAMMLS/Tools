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

app.Run("http://0.0.0.0:5001");
