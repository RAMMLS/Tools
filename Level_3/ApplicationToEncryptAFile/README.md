🚀 Features

    Secure Encryption: AES-256 CBC mode encryption with PBKDF2 key derivation

    File Browser: Built-in file system navigation

    User-Friendly GUI: Simple Windows Forms interface

    Batch Operations: Encrypt/decrypt multiple files easily

    Security Best Practices: Salt, IV, and proper key management

📋 Requirements

    Windows 10/11 or Windows Server 2016+

    .NET 6.0 Runtime or later

    Visual Studio 2022 (for development)

🛠️ Installation
Option 1: Download Pre-built Binary

    Download the latest release from [Releases Page]

    Extract the ZIP file to your desired location

    Run FileEncryptionApp.exe

📖 How to Use
Encrypting a File

    Launch the application

    Browse to the directory containing your files using the "Browse..." button

    Select a file from the list

    Enter a strong password in the password field

    Click "Encrypt File"

    The encrypted file will be saved with .encrypted extension

Decrypting a File

    Select an encrypted file (with .encrypted extension) from the list

    Enter the same password used for encryption

    Click "Decrypt File"

    The decrypted file will be saved with decrypted_ prefix

🔒 Security Features

    Algorithm: AES-256 (Advanced Encryption Standard)

    Key Derivation: PBKDF2 with 100,000 iterations

    Mode: CBC (Cipher Block Chaining)

    Padding: PKCS7

    Salt: 16-byte random salt for each encryption

    IV: 16-byte random initialization vector for each encryption
