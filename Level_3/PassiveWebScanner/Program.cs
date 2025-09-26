using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class PassiveWebScanner
{
    public static void Main()
    {
        // Пример файловой системы (10 тестовых файлов)
        string[] testFiles = {
            "request1.txt",
            "response1.txt",
            "request2.txt",
            "response2.txt",
            "request3.txt",
            "response3.txt",
            "request4.txt",
            "response4.txt",
            "request5.txt",
            "response5.txt"
        };

        // Создание тестовых файлов
        CreateTestFiles(testFiles);

        // Анализ файлов
        foreach (var file in testFiles)
        {
            Console.WriteLine($"Анализ файла: {file}");
            AnalyzeFile(file);
            Console.WriteLine(new string('-', 40));
        }
    }

    public static void AnalyzeFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не существует!");
            return;
        }

        string content = File.ReadAllText(filePath);

        // Проверки безопасности
        CheckForSensitiveData(content);
        CheckForCommonVulnerabilities(content);
        CheckHeadersSecurity(content);
    }

    private static void CheckForSensitiveData(string content)
    {
        var patterns = new Dictionary<string, string>
        {
            { @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", "Email" },
            { @"\b\d{3}-\d{2}-\d{4}\b", "SSN" },
            { @"\b(?:\d{4}[- ]?){3}\d{4}\b", "Credit Card" }
        };

        foreach (var (pattern, description) in patterns)
        {
            var matches = Regex.Matches(content, pattern);
            if (matches.Count > 0)
            {
                Console.WriteLine($"[!] Обнаружены {description}: {matches.Count}");
            }
        }
    }

    private static void CheckForCommonVulnerabilities(string content)
    {
        // Проверка SQL-инъекций
        if (Regex.IsMatch(content, @"(\bUNION\b.*\bSELECT\b|\bDROP\b|\bINSERT\b|\bDELETE\b)", RegexOptions.IgnoreCase))
        {
            Console.WriteLine("[!] Возможная SQL-инъекция");
        }

        // Проверка XSS
        if (Regex.IsMatch(content, @"<script>|javascript:", RegexOptions.IgnoreCase))
        {
            Console.WriteLine("[!] Возможный XSS");
        }
    }

    private static void CheckHeadersSecurity(string content)
    {
        if (content.Contains("Server:") && !content.Contains("Server: Unknown"))
        {
            Console.WriteLine("[!] Раскрытие информации о сервере");
        }

        if (content.Contains("Set-Cookie:") && !content.Contains("HttpOnly"))
        {
            Console.WriteLine("[!] Cookie без флага HttpOnly");
        }
    }

    private static void CreateTestFiles(string[] files)
    {
        var testContent = new Dictionary<string, string>
        {
            {
                "request1.txt",
                "GET /login.php?user=admin' OR '1'='1&pass=test HTTP/1.1\nHost: example.com"
            },
            {
                "response1.txt",
                "HTTP/1.1 200 OK\nServer: Apache/2.4.7\nSet-Cookie: session=abc123\nContent-Type: text/html"
            },
            {
                "request2.txt",
                "POST /search.php HTTP/1.1\nHost: test.com\nBody: q=<script>alert('xss')</script>"
            },
            {
                "response2.txt",
                "HTTP/1.1 200 OK\nContent-Type: application/json\nData: {\"email\":\"user@example.com\",\"ssn\":\"123-45-6789\"}"
            },
            {
                "request3.txt",
                "GET /download?file=../../../etc/passwd HTTP/1.1\nHost: target.com"
            },
            {
                "response3.txt",
                "HTTP/1.1 200 OK\nSet-Cookie: id=1; HttpOnly\nContent-Type: text/html"
            },
            {
                "request4.txt",
                "POST /api/users HTTP/1.1\nHost: api.com\nBody: {\"credit_card\":\"4111-1111-1111-1111\"}"
            },
            {
                "response4.txt",
                "HTTP/1.1 301 Moved Permanently\nLocation: http://malicious.com"
            },
            {
                "request5.txt",
                "GET / HTTP/1.1\nHost: example.com\nUser-Agent: Mozilla/5.0"
            },
            {
                "response5.txt",
                "HTTP/1.1 200 OK\nContent-Type: text/html\nBody: <html>Normal page</html>"
            }
        };

        foreach (var file in files)
        {
            File.WriteAllText(file, testContent[file]);
        }
    }
}
