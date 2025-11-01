using System.Net;
using SimpleFirewall.Models;
using System.Text.Json;

namespace SimpleFirewall.Services
{
    public class WebInterface : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly RuleManager _ruleManager;
        private readonly LogService _logService;
        private readonly NetworkMonitor _networkMonitor;
        private bool _isRunning;
        private readonly Thread _listenerThread;

        public WebInterface(RuleManager ruleManager, LogService logService, NetworkMonitor networkMonitor, int port = 8080)
        {
            _ruleManager = ruleManager;
            _logService = logService;
            _networkMonitor = networkMonitor;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{port}/");
            _listenerThread = new Thread(Listen);
        }

        public void Start()
        {
            if (_isRunning) return;

            try
            {
                _listener.Start();
                _isRunning = true;
                _listenerThread.Start();
                _logService.LogInfo($"Web interface started on http://localhost:{_listener.Prefixes.First().Split(':')[2].Trim('/')}");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to start web interface: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            _logService.LogInfo("Web interface stopped");
        }

        private void Listen()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    _ = Task.Run(() => ProcessRequest(context));
                }
                catch (HttpListenerException)
                {
                    // Listener was stopped
                    break;
                }
                catch (Exception ex)
                {
                    _logService.LogError($"Web interface error: {ex.Message}");
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                var path = request.Url?.AbsolutePath.Trim('/') ?? "";
                var method = request.HttpMethod;

                switch (path)
                {
                    case "api/rules":
                        HandleRulesApi(request, response);
                        break;
                    case "api/logs":
                        HandleLogsApi(request, response);
                        break;
                    case "api/statistics":
                        HandleStatisticsApi(request, response);
                        break;
                    case "":
                        ServeStaticFile(response, "index.html");
                        break;
                    default:
                        ServeStaticFile(response, path);
                        break;
                }
            }
            catch (Exception ex)
            {
                SendError(response, 500, ex.Message);
            }
            finally
            {
                response.Close();
            }
        }

        private void HandleRulesApi(HttpListenerRequest request, HttpListenerResponse response)
        {
            var rules = _ruleManager.GetAllRules();
            var json = JsonSerializer.Serialize(rules, new JsonSerializerOptions { WriteIndented = true });
            SendJson(response, json);
        }

        private void HandleLogsApi(HttpListenerRequest request, HttpListenerResponse response)
        {
            var logs = _logService.GetRecentLogs(50);
            var json = JsonSerializer.Serialize(logs);
            SendJson(response, json);
        }

        private void HandleStatisticsApi(HttpListenerRequest request, HttpListenerResponse response)
        {
            var stats = _networkMonitor.GetStatistics();
            var json = JsonSerializer.Serialize(stats);
            SendJson(response, json);
        }

        private void ServeStaticFile(HttpListenerResponse response, string filename)
        {
            if (filename == "index.html")
            {
                var html = GenerateDashboardHtml();
                SendHtml(response, html);
            }
            else
            {
                SendError(response, 404, "Not Found");
            }
        }

        private string GenerateDashboardHtml()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <title>Simple Firewall</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; }
        .card { background: white; padding: 20px; margin: 10px 0; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 10px; }
        .stat-item { text-align: center; padding: 10px; }
        .log-entry { font-family: monospace; font-size: 12px; padding: 2px 0; }
        .allow { color: green; }
        .block { color: red; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔥 Simple Firewall Dashboard</h1>
        
        <div class='card'>
            <h2>Statistics</h2>
            <div id='stats' class='stats'></div>
        </div>

        <div class='card'>
            <h2>Firewall Rules</h2>
            <div id='rules'></div>
        </div>

        <div class='card'>
            <h2>Recent Logs</h2>
            <div id='logs'></div>
        </div>
    </div>

    <script>
        async function loadData() {
            await loadStats();
            await loadRules();
            await loadLogs();
        }

        async function loadStats() {
            const response = await fetch('/api/statistics');
            const stats = await response.json();
            document.getElementById('stats').innerHTML = stats.map(s => 
                `<div class='stat-item'>
                    <h3>${s.ruleName}</h3>
                    <div>Allowed: ${s.packetsAllowed} packets</div>
                    <div>Blocked: ${s.packetsBlocked} packets</div>
                </div>`
            ).join('');
        }

        async function loadRules() {
            const response = await fetch('/api/rules');
            const rules = await response.json();
            document.getElementById('rules').innerHTML = rules.map(rule => 
                `<div style='margin: 5px 0; padding: 10px; border-left: 4px solid ${rule.action === 'Allow' ? 'green' : 'red'};'>
                    <strong>${rule.name}</strong> - ${rule.description}<br>
                    <small>${rule.direction} | Priority: ${rule.priority} | ${rule.isEnabled ? 'Enabled' : 'Disabled'}</small>
                </div>`
            ).join('');
        }

        async function loadLogs() {
            const response = await fetch('/api/logs');
            const logs = await response.json();
            document.getElementById('logs').innerHTML = logs.map(log => 
                `<div class='log-entry ${log.includes('[BLOCK]') ? 'block' : 'allow'}'>${log}</div>`
            ).join('');
        }

        // Auto-refresh every 5 seconds
        setInterval(loadData, 5000);
        loadData();
    </script>
</body>
</html>";
        }

        private void SendJson(HttpListenerResponse response, string json)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void SendHtml(HttpListenerResponse response, string html)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(html);
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void SendError(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            var error = new { error = message };
            var json = JsonSerializer.Serialize(error);
            SendJson(response, json);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
