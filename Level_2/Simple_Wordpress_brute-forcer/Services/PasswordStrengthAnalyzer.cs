using System.Text.RegularExpressions;

namespace WordPressSecurityTrainingTool.Services
{
    public class PasswordStrengthAnalyzer
    {
        public PasswordStrengthResult TestPasswordStrength(string password)
        {
            int score = 0;
            string recommendation = "";
            
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            
            if (Regex.IsMatch(password, @"[a-z]") && Regex.IsMatch(password, @"[A-Z]")) 
                score++;
            
            if (Regex.IsMatch(password, @"[0-9]")) 
                score++;
            
            if (Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]")) 
                score++;
            
            // Определение рекомендации
            if (score < 3)
            {
                recommendation = "Очень слабый пароль. Используйте не менее 12 символов, разные регистры, цифры и специальные символы.";
            }
            else if (score < 5)
            {
                recommendation = "Средний пароль. Добавьте специальные символы и увеличьте длину.";
            }
            else
            {
                recommendation = "Сильный пароль. Рекомендуется использовать менеджер паролей для хранения.";
            }
            
            return new PasswordStrengthResult
            {
                Score = score,
                Recommendation = recommendation
            };
        }
    }
    
    public class PasswordStrengthResult
    {
        public int Score { get; set; }
        public string Recommendation { get; set; }
    }
}
