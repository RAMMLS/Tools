namespace WordPressSecurityTrainingTool.Models
{
    public class TrainingSessionConfig
    {
        public string[] AllowedTestDomains { get; set; }
        public int RequestDelayMs { get; set; }
        public int MaxDemoAttempts { get; set; }
        public string LogFilePath { get; set; }
        public bool EnableDetailedLogging { get; set; }
    }
}
