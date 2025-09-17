using System;
using System.Threading.Tasks;

namespace WordPressSecurityTrainingTool.Utilities
{
    public class RateLimiter
    {
        private readonly TimeSpan _interval;
        private DateTime _lastRequest = DateTime.MinValue;

        public RateLimiter(TimeSpan interval)
        {
            _interval = interval;
        }

        public async Task WaitAsync()
        {
            var timeSinceLast = DateTime.UtcNow - _lastRequest;
            if (timeSinceLast < _interval)
            {
                await Task.Delay(_interval - timeSinceLast);
            }
            _lastRequest = DateTime.UtcNow;
        }
    }
}
