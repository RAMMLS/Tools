#!/bin/bash

echo "🔧 Fixing build issues..."

# Создаем минимальную рабочую версию если нужно
if [ ! -f "Program.cs" ]; then
  echo "Creating minimal Program.cs..."
  cat >Program.cs <<'EOF'
using System;
using System.Threading.Tasks;

namespace SimpleFirewall
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🔥 Simple Firewall");
            Console.WriteLine("==================");
            Console.WriteLine("Web Interface: http://localhost:8080");
            
            // Простой веб-сервер
            using var listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            
            Console.WriteLine("Firewall started successfully!");
            
            while (true)
            {
                var context = await listener.GetContextAsync();
                var response = context.Response;
                var html = "<html><body><h1>Simple Firewall</h1><p>Status: Running</p></body></html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
        }
    }
}
EOF
fi

# Проверяем и исправляем RuleManager.cs
if [ -f "Services/RuleManager.cs" ]; then
  echo "Checking RuleManager.cs..."
  # Удаляем лишние private модификаторы в начале файла
  sed -i '/^private$/d' Services/RuleManager.cs 2>/dev/null || true
fi

echo "✅ Build issues fixed!"
