using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

public class ProxyManager
{
    private readonly List<WebProxy> _proxies = new List<WebProxy>();
    private int _currentProxyIndex = 0;
    private readonly string _proxyFilePath;
    private readonly ILogger _logger;

    public ProxyManager(string proxyFilePath, ILogger logger)
    {
        _proxyFilePath = proxyFilePath ?? throw new ArgumentNullException(nameof(proxyFilePath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        LoadProxies();
    }

    private void LoadProxies()
    {
        try
        {
            if (File.Exists(_proxyFilePath))
            {
                string[] lines = File.ReadAllLines(_proxyFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        string host = parts[0];
                        int port = int.Parse(parts[1]);
                        WebProxy proxy;

                        if (parts.Length == 4)
                        {
                            // Proxy with username and password
                            string username = parts[2];
                            string password = parts[3];
                            proxy = new WebProxy(host, port)
                            {
                                Credentials = new NetworkCredential(username, password)
                            };
                        }
                        else
                        {
                            // Simple proxy without authentication
                            proxy = new WebProxy(host, port);
                        }

                        _proxies.Add(proxy);
                         _logger.LogInformation($"Загружен прокси: {host}:{port}");
                    }
                }
            }
            else
            {
                _logger.LogWarning($"Файл прокси не найден: {_proxyFilePath}");
            }
        }
        catch (Exception ex)
        {
             _logger.LogError($"Ошибка при загрузке прокси: {ex.Message}");
        }
    }

    public WebProxy GetNextProxy()
    {
        if (_proxies.Count == 0)
        {
            return null; // No proxies available
        }

        WebProxy proxy = _proxies[_currentProxyIndex];
        _currentProxyIndex = (_currentProxyIndex + 1) % _proxies.Count; // Cycle through proxies
        return proxy;
    }
}

