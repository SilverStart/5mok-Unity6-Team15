using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;

public static class AESCrypto
{
    // 암호화 키
    private static readonly string privateKey = "MySecretKey_TeamProject_12345678";

    // 암호화 함수 (평문 -> 암호문)
    public static string Encrypt(string plainText)
    {
        try
        {
            byte[] key = Encoding.UTF8.GetBytes(privateKey[..32]); // 32바이트 키 사용
            byte[] iv = Encoding.UTF8.GetBytes(privateKey[..16]);  // 16바이트 IV 사용

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        catch (Exception e)
        {
            Debug.LogError($"암호화 오류: {e.Message}");
            return null;
        }
    }

    // 복호화 함수 (암호문 -> 평문)
    public static string Decrypt(string cipherText)
    {
        try
        {
            byte[] key = Encoding.UTF8.GetBytes(privateKey[..32]);
            byte[] iv = Encoding.UTF8.GetBytes(privateKey[..16]);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(cipherBytes);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogError($"복호화 오류: {e.Message}");
            return null;
        }
    }
}