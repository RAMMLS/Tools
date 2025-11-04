using Gateway.Models;

namespace Gateway.Services
{
    public interface ILoadBalancer
    {
        ServiceEndpoint? GetNextEndpoint(List<ServiceEndpoint> endpoints);
    }

    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private int _currentIndex = -1;
        private readonly object _lock = new();

        public ServiceEndpoint? GetNextEndpoint(List<ServiceEndpoint> endpoints)
        {
            if (endpoints == null || endpoints.Count == 0)
                return null;

            var healthyEndpoints = endpoints.Where(e => e.IsHealthy).ToList();
            if (healthyEndpoints.Count == 0)
                return null;

            lock (_lock)
            {
                _currentIndex = (_currentIndex + 1) % healthyEndpoints.Count;
                return healthyEndpoints[_currentIndex];
            }
        }
    }

    public class WeightedLoadBalancer : ILoadBalancer
    {
        private readonly Random _random = new();

        public ServiceEndpoint? GetNextEndpoint(List<ServiceEndpoint> endpoints)
        {
            var healthyEndpoints = endpoints.Where(e => e.IsHealthy).ToList();
            if (healthyEndpoints.Count == 0)
                return null;

            var totalWeight = healthyEndpoints.Sum(e => e.Weight);
            var randomValue = _random.Next(totalWeight);
            var currentWeight = 0;

            foreach (var endpoint in healthyEndpoints)
            {
                currentWeight += endpoint.Weight;
                if (randomValue < currentWeight)
                    return endpoint;
            }

            return healthyEndpoints.First();
        }
    }

    public class LoadBalancerFactory
    {
        public static ILoadBalancer Create(string strategy)
        {
            return strategy.ToLower() switch
            {
                "weighted" => new WeightedLoadBalancer(),
                "roundrobin" => new RoundRobinLoadBalancer(),
                _ => new RoundRobinLoadBalancer()
            };
        }
    }
}
