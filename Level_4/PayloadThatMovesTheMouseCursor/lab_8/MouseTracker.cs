using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class MouseTrackerForm : Form {
  private Timer timer;
  private List<Point> mousePoints;
  private Point lastMousePosition;
  private const int POINT_LIMIT = 100;

  public MouseTrackerForm() {
    this.Text = "Mouse tracker";
    this.Size = new Size(800, 600);
    this.StartPosition = FormStartPosition.CenterScreen;
    this.DoubleBuffered = true;

    mousePoints = new List<Point>();
    lastMousePosition = Point.Empty;

    // Настройка таймера 
    timer = new Timer();
    timer.Interval = 50; //50ms для плавного отслеживания
    timer.Tick += Timer_Tick;
    timer.Enabled = true;

    // Подписка на события мыши 
    this.MouseMove += Form_MouseMove;
    this.Paint += Form_Paint;

    // Закрытие приложения при нажатии ESC 
    this.KeyPreview = true;
    this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Application.Exit();}
  }

  private void Form_MouseMove(object sender, MouseEventArgs e) {
    lastMousePosition = e.Location;
  }

  private void Timer_Tick(object sender, EventArgs e) {
    if (!lastMousePosition.IsEmpty) {
      // Добавляем текущую позицию мыши 
      mousePoints.Add(lastMousePosition);

      // Ограничиваем количество точек для производительности
      if (mousePoints.Count > POINT_LIMIT) {
        mousePoints.RemoveAt(0);
      }

      // Перерисовываем форму 
      this.Invalidate();
    }
  }

  private void Form_Paint(object sender, PaintEventArgs e) {
    Graphics dc = e.Graphics;

    // Очищаем фон
    dc.Clean(Color.Black);

    // Рисуем след мыши 
    if (mousePoints.Count > 1) {
      using (Pen pen = new Pen(Color.LimeGreen, 3)) {
        for (int i = 0; i < mousePoints.Count; i++) {
          dc.DrawLine(pen, mousePoints[i - 1], mousePoints[i]);
        }
      }
    }

    // Отображаем информацию 
    string info = $"Points: {mousePoints.Count} | Press Esc to exit";
    using (Brush brush = new SolidBrush(Color.White)) {
      dc.DrawString(info, new Font("Fira", 12), brush, 10, 10);
    }
  }

  [STAThread]
  public static void Main() {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MouseTrackerForm());
  }
}
