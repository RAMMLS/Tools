#!/bin/bash

echo "🚀 Setting up Gateway project..."

# Create directories
mkdir -p gateway service-a service-b

echo "✅ Directories created"

# Create .csproj files to avoid dotnet new command
cat >service-a/ServiceA.csproj <<'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
EOF

cat >service-b/ServiceB.csproj <<'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
EOF

cat >gateway/Gateway.csproj <<'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
EOF

# Create Program.cs files
cat >service-a/Program.cs <<'EOF'
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
EOF

cat >service-b/Program.cs <<'EOF'
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
EOF

cat >gateway/Program.cs <<'EOF'
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
EOF

echo "✅ Project files created"
echo "📁 Project structure:"
find . -type f -name "*.cs" -o -name "*.csproj" | sort

echo "🎯 Now run: make build"
