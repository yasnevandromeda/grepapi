using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class StringEncryptor
{
    private readonly byte[] key;
    private readonly byte[] iv;

    public StringEncryptor(string keyString, string ivString = "435345")
    {
        byte[] saltBytes = Encoding.UTF8.GetBytes(ivString);
        var keyBytes = new Rfc2898DeriveBytes(keyString, saltBytes, 10000);
        key = keyBytes.GetBytes(32);
        iv = keyBytes.GetBytes(16);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}