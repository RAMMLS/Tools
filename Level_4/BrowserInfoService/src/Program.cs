var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<BrowserInfoService.Services.IBrowserInfoService, BrowserInfoService.Services.BrowserInfoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Browser Info Service is running. Use /browserinfo endpoint.");

app.Run();
