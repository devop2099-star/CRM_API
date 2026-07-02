using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CRM.ApiHub.Infrastructure.Persistence;

public static class EncryptionHelper
{
    // Clave de 32 bytes y IV de 16 bytes para cifrado simétrico AES-256
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("NyxCRMDatabaseKeySecret2026Secur"); // 32 bytes
    private static readonly byte[] Iv = Encoding.UTF8.GetBytes("NyxCRMInitVector"); // 16 bytes

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
        }

        return "Encrypted:" + Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;
        if (!cipherText.StartsWith("Encrypted:")) return cipherText;

        var cleanCipherText = cipherText.Substring(10);
        var cipherBytes = Convert.FromBase64String(cleanCipherText);

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = Iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
