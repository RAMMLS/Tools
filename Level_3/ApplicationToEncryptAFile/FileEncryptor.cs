using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileEncryptionApp
{
    public static class FileEncryptor
    {
        // Размер соли для усиления безопасности
        private static readonly int SaltSize = 16;
        // Размер вектора инициализации
        private static readonly int IvSize = 16;
        // Размер ключа
        private static readonly int KeySize = 32;
        // Количество итераций для PBKDF2
        private static readonly int Iterations = 100000;

        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                // Генерируем случайную соль
                byte[] salt = GenerateRandomBytes(SaltSize);
                // Генерируем случайный вектор инициализации
                byte[] iv = GenerateRandomBytes(IvSize);
                
                // Создаем ключ из пароля
                using (var keyGenerator = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] key = keyGenerator.GetBytes(KeySize);
                    
                    // Создаем AES шифровальщик
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        
                        using (var outputStream = new FileStream(outputFile, FileMode.Create))
                        {
                            // Записываем соль и IV в начало файла
                            outputStream.Write(salt, 0, salt.Length);
                            outputStream.Write(iv, 0, iv.Length);
                            
                            // Создаем криптопоток
                            using (var cryptoStream = new CryptoStream(outputStream, 
                                aes.CreateEncryptor(), 
                                CryptoStreamMode.Write))
                            {
                                // Шифруем данные из исходного файла
                                using (var inputStream = new FileStream(inputFile, FileMode.Open))
                                {
                                    inputStream.CopyTo(cryptoStream);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Encryption error: {ex.Message}", ex);
            }
        }

        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                using (var inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    // Читаем соль и IV из зашифрованного файла
                    byte[] salt = new byte[SaltSize];
                    inputStream.Read(salt, 0, SaltSize);
                    
                    byte[] iv = new byte[IvSize];
                    inputStream.Read(iv, 0, IvSize);
                    
                    // Создаем ключ из пароля
                    using (var keyGenerator = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                    {
                        byte[] key = keyGenerator.GetBytes(KeySize);
                        
                        // Создаем AES дешифровальщик
                        using (Aes aes = Aes.Create())
                        {
                            aes.Key = key;
                            aes.IV = iv;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;
                            
                            // Создаем криптопоток для дешифрования
                            using (var cryptoStream = new CryptoStream(inputStream, 
                                aes.CreateDecryptor(), 
                                CryptoStreamMode.Read))
                            {
                                // Записываем дешифрованные данные в выходной файл
                                using (var outputStream = new FileStream(outputFile, FileMode.Create))
                                {
                                    cryptoStream.CopyTo(outputStream);
                                }
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                throw new Exception("Decryption failed. Wrong password or corrupted file.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Decryption error: {ex.Message}", ex);
            }
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
