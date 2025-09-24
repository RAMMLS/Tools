using System;

namespace ScreenshotCaptureTool
{
    public class CaptureSettings
    {
        public int CaptureIntervalMs { get; set; } = 30000; // 30 секунд
        public string OutputFormat { get; set; } = "JPEG";
        public int Quality { get; set; } = 85;
        public bool CaptureAllScreens { get; set; } = false;
    }
    
    public class Config
    {
        public CaptureSettings LoadSettings()
        {
            // Здесь может быть загрузка из конфиг файла
            return new CaptureSettings();
        }
        
        public void SaveSettings(CaptureSettings settings)
        {
            // Сохранение настроек
        }
    }
}
