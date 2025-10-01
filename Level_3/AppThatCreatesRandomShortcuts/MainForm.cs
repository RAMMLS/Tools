using System;
using System.Drawing;
using System.Windows.Forms;
using RandomShortcutCreator.Models;
using RandomShortcutCreator.Services;

namespace RandomShortcutCreator
{
    public partial class MainForm : Form
    {
        private readonly ApplicationSettings _settings;
        private readonly ShortcutService _shortcutService;
        private readonly RandomGenerator _randomGenerator;

        public MainForm(ApplicationSettings settings)
        {
            _settings = settings;
            _shortcutService = new ShortcutService();
            _randomGenerator = new RandomGenerator();
            
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Random Shortcut Creator v1.0";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            CreateControls();
            LoadSettings();
        }

        private void CreateControls()
        {
            // Панель управления
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.LightGray
            };

            // Элементы управления
            var lblCount = new Label { Text = "Number of shortcuts:", Location = new Point(20, 20), AutoSize = true };
            var numCount = new NumericUpDown { Value = 5, Minimum = 1, Maximum = 50, Location = new Point(150, 18), Width = 60 };

            var lblTarget = new Label { Text = "Target type:", Location = new Point(20, 50), AutoSize = true };
            var cmbTarget = new ComboBox { 
                Location = new Point(150, 48), 
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTarget.Items.AddRange(new[] { "Application", "Website", "Document", "Folder" });
            cmbTarget.SelectedIndex = 0;

            var chkRandomNames = new CheckBox { Text = "Use random names", Location = new Point(20, 80), AutoSize = true, Checked = true };
            var chkRandomIcons = new CheckBox { Text = "Use random icons", Location = new Point(20, 110), AutoSize = true, Checked = true };
            var chkDesktopOnly = new CheckBox { Text = "Create on desktop only", Location = new Point(150, 80), AutoSize = true, Checked = true };

            var btnCreate = new Button { Text = "Create Shortcuts", Location = new Point(20, 150), Size = new Size(120, 30) };
            var btnClear = new Button { Text = "Clear All", Location = new Point(150, 150), Size = new Size(80, 30) };
            var btnSettings = new Button { Text = "Settings", Location = new Point(240, 150), Size = new Size(80, 30) };

            // Текстовое поле для логов
            var txtLog = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            // Добавление элементов на панель
            panel.Controls.AddRange(new Control[] 
            { 
                lblCount, numCount, lblTarget, cmbTarget,
                chkRandomNames, chkRandomIcons, chkDesktopOnly,
                btnCreate, btnClear, btnSettings
            });

            // Добавление на форму
            this.Controls.Add(panel);
            this.Controls.Add(txtLog);

            // Обработчики событий
            btnCreate.Click += (s, e) => CreateShortcuts(
                (int)numCount.Value,
                cmbTarget.SelectedItem.ToString(),
                chkRandomNames.Checked,
                chkRandomIcons.Checked,
                chkDesktopOnly.Checked,
                txtLog);

            btnClear.Click += (s, e) => ClearShortcuts(txtLog);
            btnSettings.Click += (s, e) => ShowSettings();
        }

        private void CreateShortcuts(int count, string targetType, bool randomNames, bool randomIcons, bool desktopOnly, TextBox log)
        {
            try
            {
                log.AppendText($"Creating {count} {targetType} shortcuts...\r\n");

                var config = new ShortcutConfig
                {
                    Count = count,
                    TargetType = targetType,
                    UseRandomNames = randomNames,
                    UseRandomIcons = randomIcons,
                    DesktopOnly = desktopOnly
                };

                var results = _shortcutService.CreateRandomShortcuts(config);

                foreach (var result in results)
                {
                    var status = result.Success ? "SUCCESS" : "FAILED";
                    log.AppendText($"{status}: {result.FilePath} -> {result.TargetPath}\r\n");
                }

                log.AppendText($"Completed! Created {results.Count} shortcuts.\r\n\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shortcuts: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearShortcuts(TextBox log)
        {
            try
            {
                var result = _shortcutService.ClearCreatedShortcuts();
                log.AppendText($"Cleared {result} shortcuts.\r\n\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing shortcuts: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSettings()
        {
            var settingsForm = new SettingsForm(_settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _settings.Save();
            }
        }

        private void LoadSettings()
        {
            // Загрузка настроек в UI
        }
    }

    // Форма настроек (упрощенная версия)
    public class SettingsForm : Form
    {
        private readonly ApplicationSettings _settings;

        public SettingsForm(ApplicationSettings settings)
        {
            _settings = settings;
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Application Settings";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            // Реализация формы настроек
            // ...
        }
    }
}
