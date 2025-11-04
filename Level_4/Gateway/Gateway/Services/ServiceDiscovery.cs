using System.Net;
using Gateway.Models;

namespace Gateway.Services
{
    public class ServiceDiscovery
    {
        private readonly HttpClient _httpClient;
        private readonly RouteManager _routeManager;
        private readonly Timer _healthCheckTimer;

        public ServiceDiscovery(RouteManager routeManager)
        {
            _routeManager = routeManager;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            _healthCheckTimer = new Timer(CheckServicesHealth, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public async void CheckServicesHealth(object? state)
        {
            var allRoutes = _routeManager.GetAllRoutes();
            var allEndpoints = allRoutes.SelectMany(r => r.Endpoints).Distinct().ToList();

            var healthCheckTasks = allEndpoints.Select(async endpoint =>
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{endpoint.Url}/health");
                    endpoint.IsHealthy = response.StatusCode == HttpStatusCode.OK;
                    endpoint.LastHealthCheck = DateTime.Now;
                }
                catch
                {
                    endpoint.IsHealthy = false;
                    endpoint.LastHealthCheck = DateTime.Now;
                }
            });

            await Task.WhenAll(healthCheckTasks);
        }

        public List<ServiceEndpoint> GetHealthyEndpoints(string routeId)
        {
            var route = _routeManager.GetAllRoutes().FirstOrDefault(r => r.RouteId == routeId);
            return route?.Endpoints.Where(e => e.IsHealthy).ToList() ?? new List<ServiceEndpoint>();
        }

        public void Dispose()
        {
            _healthCheckTimer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}
