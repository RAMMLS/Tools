using System;
using System.Text;

public class VigenereCipher
{
    private readonly string _key;

    public VigenereCipher(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Ключ не может быть пустым");
        
        // Проверяем, что ключ содержит только буквы
        foreach (char c in key)
        {
            if (!char.IsLetter(c))
                throw new ArgumentException("Ключ должен содержать только буквы");
        }
        
        _key = key.ToUpper();
    }

    public string Encrypt(string plainText)
    {
        return Process(plainText, true);
    }

    public string Decrypt(string cipherText)
    {
        return Process(cipherText, false);
    }

    private string Process(string text, bool encrypt)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        StringBuilder result = new StringBuilder();
        int keyIndex = 0;

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                // Определяем базовый символ для регистра
                char baseChar = char.IsUpper(c) ? 'A' : 'a';
                
                // Получаем символ ключа и преобразуем к числу 0-25
                int keyShift = _key[keyIndex % _key.Length] - 'A';
                
                // Преобразуем текущий символ текста к числу 0-25
                int textChar = c - baseChar;
                
                // Выполняем сдвиг (вперед для шифрования, назад для дешифрования)
                int processedChar = encrypt 
                    ? (textChar + keyShift) % 26 
                    : (textChar - keyShift + 26) % 26;
                
                result.Append((char)(processedChar + baseChar));
                
                // Переходим к следующему символу ключа
                keyIndex++;
            }
            else
            {
                // Не-буквенные символы добавляем без изменений
                result.Append(c);
            }
        }

        return result.ToString();
    }
}

// Пример использования
public class Program
{
    public static void Main()
    {
        var cipher = new VigenereCipher("KEY");
        
        string original = "Hello, World!";
        string encrypted = cipher.Encrypt(original);
        string decrypted = cipher.Decrypt(encrypted);
        
        Console.WriteLine($"Original: {original}");
        Console.WriteLine($"Encrypted: {encrypted}");
        Console.WriteLine($"Decrypted: {decrypted}");
    }
}
