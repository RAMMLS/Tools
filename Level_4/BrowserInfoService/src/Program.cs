using BrowserInfoService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<IBrowserInfoService, BrowserInfoCollector>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Browser Info Service is running. Use /browserinfo endpoint.");
app.MapGet("/error", () => "An error occurred. Please check the service logs.");

// Явно указываем порт 80 для Docker
app.Run("http://0.0.0.0:80");
