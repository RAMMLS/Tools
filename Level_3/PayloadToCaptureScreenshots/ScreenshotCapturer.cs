using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScreenshotCaptureTool
{
    public class ScreenshotCapturer
    {
        public Bitmap Capture()
        {
            try
            {
                // Получение размеров основного экрана
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                
                // Создание bitmap для захвата
                Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                
                using (Graphics graphics = Graphics.FromImage(screenshot))
                {
                    graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }
                
                return screenshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Capture failed: {ex.Message}");
                return null;
            }
        }
        
        public Bitmap CaptureSpecificScreen(int screenIndex)
        {
            if (screenIndex >= 0 && screenIndex < Screen.AllScreens.Length)
            {
                Rectangle bounds = Screen.AllScreens[screenIndex].Bounds;
                Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                
                using (Graphics graphics = Graphics.FromImage(screenshot))
                {
                    graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }
                
                return screenshot;
            }
            return null;
        }
    }
}
