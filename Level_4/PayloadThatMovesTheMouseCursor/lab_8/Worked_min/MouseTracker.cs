using System;
using System.Drawing;
using System.Windows.Forms;

public class SimpleMouseTracker : Form
{
    private Point lastMousePos;
    
    public SimpleMouseTracker()
    {
        this.Text = "Mouse Tracker - Press ESC to exit";
        this.Size = new Size(600, 400);
        this.BackColor = Color.Black;
        this.DoubleBuffered = true;
        
        this.MouseMove += OnMouseMove;
        this.Paint += OnPaint;
        this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Application.Exit(); };
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        lastMousePos = e.Location;
        this.Invalidate(); // Перерисовываем
    }
    
    private void OnPaint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        
        // Рисуем крестик в позиции мыши
        if (!lastMousePos.IsEmpty)
        {
            using (Pen pen = new Pen(Color.Lime, 2))
            {
                g.DrawLine(pen, lastMousePos.X - 10, lastMousePos.Y, lastMousePos.X + 10, lastMousePos.Y);
                g.DrawLine(pen, lastMousePos.X, lastMousePos.Y - 10, lastMousePos.X, lastMousePos.Y + 10);
            }
            
            // Отображаем координаты
            string coords = $"X: {lastMousePos.X}, Y: {lastMousePos.Y}";
            g.DrawString(coords, new Font("Arial", 12), Brushes.White, 10, 10);
        }
    }
    
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new SimpleMouseTracker());
    }
}
