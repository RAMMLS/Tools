using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotCaptureTool
{
    class Program
    {
        private static readonly ScreenshotCapturer capturer = new ScreenshotCapturer();
        private static readonly FileSystemManager fileManager = new FileSystemManager();
        private static readonly Config config = new Config();
        
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Screenshot Capture Tool - Security Research");
                
                // Конфигурация
                var settings = config.LoadSettings();
                
                // Захват скриншотов в фоновом режиме
                await StartCaptureSession(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        static async Task StartCaptureSession(CaptureSettings settings)
        {
            while (true)
            {
                try
                {
                    await CaptureAndSaveScreenshot();
                    await Task.Delay(settings.CaptureIntervalMs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Capture error: {ex.Message}");
                    await Task.Delay(5000);
                }
            }
        }
        
        static async Task CaptureAndSaveScreenshot()
        {
            var screenshot = capturer.Capture();
            if (screenshot != null)
            {
                string filePath = fileManager.SaveScreenshot(screenshot);
                Console.WriteLine($"Screenshot saved: {filePath}");
                
                // Очистка памяти
                screenshot.Dispose();
            }
        }
    }
}
