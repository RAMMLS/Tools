using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace WordPressSecurityTrainingTool.Utilities
{
    public static class EnvironmentChecker
    {
        public static bool IsTestEnvironment()
        {
            try
            {
                // Проверка домена на принадлежность к тестовым средам
                var allowedDomains = ConfigurationManager.AppSettings["AllowedTestDomains"]?
                    .Split(',') ?? new[] { "test.", "localhost", "127.0.0.1", "::1" };
                
                var hostName = Dns.GetHostName();
                var hostEntry = Dns.GetHostEntry(hostName);
                
                bool isTestDomain = allowedDomains.Any(domain => 
                    hostName.Contains(domain) || 
                    hostEntry.AddressList.Any(ip => ip.ToString().Contains(domain)));
                
                // Дополнительная проверка - наличие тестового файла или переменной среды
                bool hasTestFlag = Environment.GetEnvironmentVariable("IS_TEST_ENVIRONMENT") == "true";
                
                return isTestDomain || hasTestFlag;
            }
            catch
            {
                return false;
            }
        }
    }
}
