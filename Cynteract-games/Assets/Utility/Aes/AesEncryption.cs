using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AesEncryption
{

    public static byte [] GetStandardKey()
    {
        if (CandyCoded.env.env.TryParseEnvironmentVariable("AES_KEY", out string key))
        {
            return key.Split(':').Select(item => byte.Parse(item)).ToArray();
        }
        else
        {
            throw new Exception("Environment key AES_KEY not found");
        }

    }
    public static byte[] GetStandardIV()
    {
        if (CandyCoded.env.env.TryParseEnvironmentVariable("AES_IV", out string key))
        {
            return key.Split(':').Select(item => byte.Parse(item)).ToArray();
        }
        else
        {
            throw new Exception("Environment key AES_IV not found");
        }

    }
    public static byte[] Encrypt(string s)
    {
        return EncryptStringToBytes_Aes(s, GetStandardKey(), GetStandardIV());
    }
    public static string DecryptToString(byte[] bytes)
    {
        return DecryptStringFromBytes_Aes(bytes, GetStandardKey(), GetStandardIV());
    }
    public static byte[] Encrypt(byte[] s)
    {
        return EncryptBytesToBytes_Aes(s, GetStandardKey(), GetStandardIV());
    }
    public static byte[] DecryptToBytes(byte[] bytes)
    {
        return DecryptBytesFromBytes_Aes(bytes, GetStandardKey(), GetStandardIV());
    }
    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }
    static byte[] EncryptBytesToBytes_Aes(byte[] plainData, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainData == null || plainData.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        for (int i = 0; i < plainData.Length; i++)
                        {

                        swEncrypt.Write(plainData[i]);
                        }
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }
    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
    static byte[] DecryptBytesFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        byte[] plainBytes = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        using (var memstream = new MemoryStream())
                        {
                            srDecrypt.BaseStream.CopyTo(memstream);
                            var charByteArray = memstream.ToArray();
                            var charArray = new char[charByteArray.Length];
                            for (int i = 0; i < charArray.Length; i++)
                            {
                                charArray[i] = (char)charByteArray[i];
                            }
                            plainBytes = new byte[charArray.Length];
                            for (int i = 0; i < charArray.Length; i++)
                            {
                                plainBytes[i] =  (byte)Char.GetNumericValue( charArray[i]);
                            }
                        }
                    }
                }
            }
        }

        return plainBytes;
    }
}
