using System;
using System.Threading.Tasks;
using WordPressSecurityTrainingTool.Models;
using WordPressSecurityTrainingTool.Utilities;

namespace WordPressSecurityTrainingTool.Services
{
    public class TestWordPressClient
    {
        private readonly SecurityLogger _logger;
        private int _attemptCount;

        public TestWordPressClient()
        {
            _logger = new SecurityLogger();
            _attemptCount = 0;
        }

        public async Task<LoginAttemptResult> TestLoginAsync(string url, string username, string password)
        {
            // Симуция задержки сети
            await Task.Delay(TimeSpan.FromMilliseconds(150 + new Random().Next(100)));
            
            _attemptCount++;
            
            // Симуция блокировки после нескольких попыток
            bool isLockoutSimulated = _attemptCount > 3;
            
            var result = new LoginAttemptResult
            {
                Username = username,
                IsSuccessful = false, // Всегда false в учебных целях
                ResponseTime = TimeSpan.FromMilliseconds(150 + new Random().Next(100)),
                IsLockoutSimulated = isLockoutSimulated
            };
            
            if (isLockoutSimulated)
            {
                result.Lesson = "СИМУЛЯЦИЯ: Слишком много неудачных попыток. Аккаунт временно заблокирован.";
            }
            else
            {
                result.Lesson = "СИМУЛЯЦИЯ: Неудачная попытка входа. Используйте сложные пароли и ограничьте попытки входа.";
            }
            
            _logger.LogSecurityEvent(
                $"Учебная попытка входа: {username} на {url}", 
                "EducationalDemo");
            
            return result;
        }
    }
}
