namespace ARPPoisoningTool.Config
{
    public static class Settings
    {
        public static int ARPTimeout { get; set; } = 2000;
        public static int ScanTimeout { get; set; } = 5000;
        public static int MaxRetries { get; set; } = 3;
        public static string DefaultInterface { get; set; } = "eth0";
        public static bool EnableLogging { get; set; } = true;
        public static string LogDirectory { get; set; } = "logs";
        
        public static class Security
        {
            public static bool RequireAdmin { get; set; } = true;
            public static string[] AllowedNetworks { get; set; } = { "192.168.0.0/16", "10.0.0.0/8" };
        }
    }
}
