using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FileEncryptionApp
{
    public partial class MainForm : Form
    {
        private string currentDirectory;
        
        public MainForm()
        {
            InitializeComponent();
            currentDirectory = Environment.CurrentDirectory;
            UpdateFileList();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main Form
            this.Text = "File Encryption Application";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Directory Label
            Label dirLabel = new Label();
            dirLabel.Text = "Current Directory:";
            dirLabel.Location = new System.Drawing.Point(10, 10);
            dirLabel.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(dirLabel);
            
            // Directory TextBox
            TextBox dirTextBox = new TextBox();
            dirTextBox.Name = "txtDirectory";
            dirTextBox.Text = currentDirectory;
            dirTextBox.Location = new System.Drawing.Point(10, 35);
            dirTextBox.Size = new System.Drawing.Size(450, 20);
            dirTextBox.ReadOnly = true;
            this.Controls.Add(dirTextBox);
            
            // Browse Button
            Button browseBtn = new Button();
            browseBtn.Text = "Browse...";
            browseBtn.Location = new System.Drawing.Point(470, 33);
            browseBtn.Size = new System.Drawing.Size(100, 23);
            browseBtn.Click += BrowseFolder;
            this.Controls.Add(browseBtn);
            
            // File ListBox
            ListBox fileListBox = new ListBox();
            fileListBox.Name = "lstFiles";
            fileListBox.Location = new System.Drawing.Point(10, 70);
            fileListBox.Size = new System.Drawing.Size(560, 200);
            this.Controls.Add(fileListBox);
            
            // Password Label
            Label pwdLabel = new Label();
            pwdLabel.Text = "Encryption Password:";
            pwdLabel.Location = new System.Drawing.Point(10, 290);
            pwdLabel.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(pwdLabel);
            
            // Password TextBox
            TextBox pwdTextBox = new TextBox();
            pwdTextBox.Name = "txtPassword";
            pwdTextBox.Location = new System.Drawing.Point(10, 315);
            pwdTextBox.Size = new System.Drawing.Size(560, 20);
            pwdTextBox.PasswordChar = '*';
            this.Controls.Add(pwdTextBox);
            
            // Encrypt Button
            Button encryptBtn = new Button();
            encryptBtn.Text = "Encrypt File";
            encryptBtn.Location = new System.Drawing.Point(10, 350);
            encryptBtn.Size = new System.Drawing.Size(120, 30);
            encryptBtn.Click += EncryptFile;
            this.Controls.Add(encryptBtn);
            
            // Decrypt Button
            Button decryptBtn = new Button();
            decryptBtn.Text = "Decrypt File";
            decryptBtn.Location = new System.Drawing.Point(140, 350);
            decryptBtn.Size = new System.Drawing.Size(120, 30);
            decryptBtn.Click += DecryptFile;
            this.Controls.Add(decryptBtn);
            
            // Refresh Button
            Button refreshBtn = new Button();
            refreshBtn.Text = "Refresh List";
            refreshBtn.Location = new System.Drawing.Point(270, 350);
            refreshBtn.Size = new System.Drawing.Size(120, 30);
            refreshBtn.Click += RefreshList;
            this.Controls.Add(refreshBtn);
            
            // Status Label
            Label statusLabel = new Label();
            statusLabel.Name = "lblStatus";
            statusLabel.Text = "Ready";
            statusLabel.Location = new System.Drawing.Point(10, 400);
            statusLabel.Size = new System.Drawing.Size(560, 20);
            this.Controls.Add(statusLabel);
            
            this.ResumeLayout(false);
        }
        
        private void UpdateFileList()
        {
            ListBox fileList = (ListBox)this.Controls["lstFiles"];
            fileList.Items.Clear();
            
            try
            {
                // Get all files in current directory
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (string file in files)
                {
                    fileList.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Error reading directory: " + ex.Message);
            }
        }
        
        private void BrowseFolder(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select directory with files to encrypt";
                dialog.SelectedPath = currentDirectory;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    currentDirectory = dialog.SelectedPath;
                    ((TextBox)this.Controls["txtDirectory"]).Text = currentDirectory;
                    UpdateFileList();
                    UpdateStatus("Directory changed to: " + currentDirectory);
                }
            }
        }
        
        private void RefreshList(object sender, EventArgs e)
        {
            UpdateFileList();
            UpdateStatus("File list refreshed");
        }
        
        private void EncryptFile(object sender, EventArgs e)
        {
            ListBox fileList = (ListBox)this.Controls["lstFiles"];
            TextBox pwdTextBox = (TextBox)this.Controls["txtPassword"];
            
            if (fileList.SelectedItem == null)
            {
                MessageBox.Show("Please select a file to encrypt", "Warning", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(pwdTextBox.Text))
            {
                MessageBox.Show("Please enter encryption password", "Warning", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string fileName = fileList.SelectedItem.ToString();
            string inputFile = Path.Combine(currentDirectory, fileName);
            string outputFile = Path.Combine(currentDirectory, fileName + ".encrypted");
            
            try
            {
                FileEncryptor.EncryptFile(inputFile, outputFile, pwdTextBox.Text);
                UpdateStatus($"File encrypted successfully: {outputFile}");
                UpdateFileList();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Encryption failed: {ex.Message}");
                MessageBox.Show($"Encryption failed: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DecryptFile(object sender, EventArgs e)
        {
            ListBox fileList = (ListBox)this.Controls["lstFiles"];
            TextBox pwdTextBox = (TextBox)this.Controls["txtPassword"];
            
            if (fileList.SelectedItem == null)
            {
                MessageBox.Show("Please select a file to decrypt", "Warning", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string fileName = fileList.SelectedItem.ToString();
            if (!fileName.EndsWith(".encrypted"))
            {
                MessageBox.Show("Please select an encrypted file (.encrypted extension)", "Warning", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(pwdTextBox.Text))
            {
                MessageBox.Show("Please enter decryption password", "Warning", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string inputFile = Path.Combine(currentDirectory, fileName);
            string originalName = fileName.Replace(".encrypted", "");
            string outputFile = Path.Combine(currentDirectory, "decrypted_" + originalName);
            
            try
            {
                FileEncryptor.DecryptFile(inputFile, outputFile, pwdTextBox.Text);
                UpdateStatus($"File decrypted successfully: {outputFile}");
                UpdateFileList();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Decryption failed: {ex.Message}");
                MessageBox.Show($"Decryption failed: {ex.Message}", "Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateStatus(string message)
        {
            ((Label)this.Controls["lblStatus"]).Text = message;
        }
    }
}
