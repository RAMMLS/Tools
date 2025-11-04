namespace Gateway.Models
{
    public class RouteConfig
    {
        public string RouteId { get; set; } = Guid.NewGuid().ToString();
        public string Path { get; set; } = string.Empty;
        public string Methods { get; set; } = "GET,POST,PUT,DELETE";
        public List<ServiceEndpoint> Endpoints { get; set; } = new();
        public bool RequiresAuthentication { get; set; } = false;
        public int RateLimit { get; set; } = 100; // запросов в минуту
        public string LoadBalancingStrategy { get; set; } = "RoundRobin";
        public int Timeout { get; set; } = 30; // секунды
        public Dictionary<string, string> Headers { get; set; } = new();

        public override string ToString()
        {
            return $"{Path} → {Endpoints.Count} endpoints";
        }
    }

    public class ServiceEndpoint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Url { get; set; } = string.Empty;
        public bool IsHealthy { get; set; } = true;
        public int Weight { get; set; } = 1;
        public DateTime LastHealthCheck { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{Url} ({(IsHealthy ? "Healthy" : "Unhealthy")})";
        }
    }

    public class GatewayRequest
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public string ClientIp { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public string Body { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserAgent { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Method} {Path} from {ClientIp}";
        }
    }
}
