using System;

namespace WordPressSecurityTrainingTool.Models
{
    public class LoginAttemptResult
    {
        public string Username { get; set; }
        public bool IsSuccessful { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public string Lesson { get; set; }
        public bool IsLockoutSimulated { get; set; }
    }
}
