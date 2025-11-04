using System.Text.Json;
using Gateway.Models;

namespace Gateway.Services
{
    public class RouteManager
    {
        private readonly List<RouteConfig> _routes;
        private readonly string _configFile;
        private readonly ReaderWriterLockSlim _lock = new();

        public event Action<RouteConfig>? RouteAdded;
        public event Action<RouteConfig>? RouteUpdated;
        public event Action<string>? RouteRemoved;

        public RouteManager(string configFile = "routes.json")
        {
            _configFile = configFile;
            _routes = new List<RouteConfig>();
            LoadRoutes();
        }

        public void AddRoute(RouteConfig route)
        {
            _lock.EnterWriteLock();
            try
            {
                _routes.Add(route);
                SaveRoutes();
                RouteAdded?.Invoke(route);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool RemoveRoute(string routeId)
        {
            _lock.EnterWriteLock();
            try
            {
                var route = _routes.FirstOrDefault(r => r.RouteId == routeId);
                if (route != null)
                {
                    _routes.Remove(route);
                    SaveRoutes();
                    RouteRemoved?.Invoke(routeId);
                    return true;
                }
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void UpdateRoute(RouteConfig updatedRoute)
        {
            _lock.EnterWriteLock();
            try
            {
                var existingRoute = _routes.FirstOrDefault(r => r.RouteId == updatedRoute.RouteId);
                if (existingRoute != null)
                {
                    _routes.Remove(existingRoute);
                    _routes.Add(updatedRoute);
                    SaveRoutes();
                    RouteUpdated?.Invoke(updatedRoute);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public RouteConfig? GetRouteForPath(string path, string method)
        {
            _lock.EnterReadLock();
            try
            {
                return _routes.FirstOrDefault(route =>
                    path.StartsWith(route.Path, StringComparison.OrdinalIgnoreCase) &&
                    route.Methods.Split(',').Contains(method, StringComparer.OrdinalIgnoreCase));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<RouteConfig> GetAllRoutes()
        {
            _lock.EnterReadLock();
            try
            {
                return new List<RouteConfig>(_routes);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private void LoadRoutes()
        {
            try
            {
                if (File.Exists(_configFile))
                {
                    var json = File.ReadAllText(_configFile);
                    var routes = JsonSerializer.Deserialize<List<RouteConfig>>(json);
                    if (routes != null)
                    {
                        _routes.Clear();
                        _routes.AddRange(routes);
                    }
                }
                else
                {
                    LoadDefaultRoutes();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading routes: {ex.Message}");
                LoadDefaultRoutes();
            }
        }

        private void SaveRoutes()
        {
            try
            {
                var json = JsonSerializer.Serialize(_routes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving routes: {ex.Message}");
            }
        }

        private void LoadDefaultRoutes()
        {
            _routes.AddRange(new[]
            {
                new RouteConfig
                {
                    Path = "/api/users",
                    Methods = "GET,POST,PUT,DELETE",
                    Endpoints = new List<ServiceEndpoint>
                    {
                        new ServiceEndpoint { Url = "http://localhost:5001" },
                        new ServiceEndpoint { Url = "http://localhost:5002" }
                    },
                    RequiresAuthentication = true,
                    RateLimit = 100,
                    LoadBalancingStrategy = "RoundRobin"
                },
                new RouteConfig
                {
                    Path = "/api/products",
                    Methods = "GET,POST",
                    Endpoints = new List<ServiceEndpoint>
                    {
                        new ServiceEndpoint { Url = "http://localhost:5003" }
                    },
                    RequiresAuthentication = false,
                    RateLimit = 200,
                    LoadBalancingStrategy = "RoundRobin"
                },
                new RouteConfig
                {
                    Path = "/api/orders",
                    Methods = "GET,POST,PUT",
                    Endpoints = new List<ServiceEndpoint>
                    {
                        new ServiceEndpoint { Url = "http://localhost:5004" },
                        new ServiceEndpoint { Url = "http://localhost:5005" }
                    },
                    RequiresAuthentication = true,
                    RateLimit = 50,
                    LoadBalancingStrategy = "Weighted"
                }
            });
            SaveRoutes();
        }
    }
}
