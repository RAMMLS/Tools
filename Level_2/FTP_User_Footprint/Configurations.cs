using System;
using System.IO;

namespace FtpUserFootprint
{
    class Configuration
    {
        public string FtpServer { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }

        public static Configuration LoadConfiguration()
        {
            // This is a very basic example.  Don't store credentials like this in real apps!
            // Read from a secure configuration file or environment variables instead.

            //Create Default config file if it does not exist
            if (!File.Exists("config.txt"))
            {
                using (StreamWriter sw = File.CreateText("config.txt"))
                {
                    sw.WriteLine("FtpServer=ftp://your_ftp_server"); // Example: ftp://ftp.example.com
                    sw.WriteLine("FtpUsername=your_username");
                    sw.WriteLine("FtpPassword=your_password");
                }
                Console.WriteLine("Created config.txt with example values, please update it with real values!");
            }
            // Read configuration from config.txt file
            string[] lines = File.ReadAllLines("config.txt");
            var config = new Configuration();
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "FtpServer":
                            config.FtpServer = value;
                            break;
                        case "FtpUsername":
                            config.FtpUsername = value;
                            break;
                        case "FtpPassword":
                            config.FtpPassword = value;
                            break;
                    }
                }
            }
            return config;
        }
    }
}

