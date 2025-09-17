using System;
using System.Configuration;
using System.Linq;

namespace WordPressSecurityTrainingTool.Utilities
{
    public static class DomainValidator
    {
        public static void ValidateTargetDomain(string domain)
        {
            var allowedDomains = ConfigurationManager.AppSettings["AllowedTestDomains"]?
                .Split(',') ?? new[] { "test.", "localhost", "127.0.0.1", "::1" };
            
            bool isAllowed = allowedDomains.Any(allowed => 
                domain.Contains(allowed) || domain.Equals(allowed));
            
            if (!isAllowed)
            {
                throw new UnauthorizedAccessException(
                    $"Целевой домен '{domain}' не разрешен для тестирования. " +
                    "Разрешены только: " + string.Join(", ", allowedDomains));
            }
        }
    }
}
