using System;
using System.Windows.Forms;

namespace RandomShortcutCreator {
  internal static class Program {
    [STAThread]
    static void Mian() {
      Application.EnableVisualSTyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var settings = ApplicationSettings.Load();

      Application.Run(new MainForm(settings));
    }
  }
}
