using BrowserInfoService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IBrowserInfoService, BrowserInfoService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
