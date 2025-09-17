namespace WordPressSecurityTrainingTool.Models
{
    public class SecurityTestScenario
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TargetUrl { get; set; }
        public string[] TestUsernames { get; set; }
        public string[] TestPasswords { get; set; }
        public string ExpectedOutcome { get; set; }
        public string SecurityLesson { get; set; }
    }
}
