using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace KC.Storage.Util
{
    public static class Encryption
    {
        private const string SECRETE_ENCRYPTION_KEY = "saltIsGoodForYou";

        public static string GetEncryptionKey(string encryptionKey, bool isUserLevelBlob, bool isInternal = false, string userId = null)
        {
            if (string.IsNullOrEmpty(encryptionKey))
            {
                return null; // Do not encrypt
            }

            string encryptKey = encryptionKey;
            if (isUserLevelBlob)
            {
                encryptKey += "|" + (isInternal ? "Internal" : userId);
            }
            return encryptKey;
        }


        public static string Encrypt(string value, string key)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(value), key));
        }

        public static byte[] Encrypt(byte[] data, string key)
        {
            if (data == null) return null;

            string encryptionKey = string.IsNullOrEmpty(key)
                                       ? SECRETE_ENCRYPTION_KEY
                                       : SECRETE_ENCRYPTION_KEY + "|" + key;

            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(encryptionKey);

            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes("thePassword", saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;

            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Encryption
            ICryptoTransform encryptTransf = aes.CreateEncryptor();

            // Output stream, can be also a FileStream
            MemoryStream encryptStream = new MemoryStream();
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);

            encryptor.Write(data, 0, data.Length);
            encryptor.Flush();
            encryptor.Close();

            byte[] encryptBytes = encryptStream.ToArray();

            return encryptBytes;
        }

        public static string Encrypt(string input)
        {
            if (input == null)
                return null;

            byte[] binaryData = UTF8Encoding.UTF8.GetBytes(input);
            byte[] encryptBytes = Encrypt(binaryData, null);

            string encryptedString = Convert.ToBase64String(encryptBytes);
            return encryptedString;
        }

        public static byte[] Decrypt(byte[] data, string publicKey)
        {
            if (data == null) return null;

            try
            {
                string encryptionKey = string.IsNullOrEmpty(publicKey)
                                           ? SECRETE_ENCRYPTION_KEY
                                           : SECRETE_ENCRYPTION_KEY + "|" + publicKey;

                byte[] saltBytes = Encoding.UTF8.GetBytes(encryptionKey);

                // Our symmetric encryption algorithm
                AesManaged aes = new AesManaged();

                // We're using the PBKDF2 standard for password-based key generation
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes("thePassword", saltBytes);

                // Setting our parameters
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                // Now, decryption
                ICryptoTransform decryptTrans = aes.CreateDecryptor();

                // Output stream, can be also a FileStream
                MemoryStream decryptStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

                decryptor.Write(data, 0, data.Length);
                decryptor.Flush();
                decryptor.Close();

                // Showing our decrypted content
                byte[] decryptBytes = decryptStream.ToArray();

                return decryptBytes;
            }
            catch (Exception e)
            {
                throw new Exception("Decryption Error. There might be a malicious access." + e.Message + System.Environment.NewLine + e.StackTrace);
            }

        }

        public static string Decrypt(string base64Input)
        {
            if (base64Input == null)
                return null;

            byte[] encryptBytes = Convert.FromBase64String(base64Input);
            byte[] decryptBytes = Decrypt(encryptBytes, null);

            string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            return decryptedString;
        }

        public static string Decrypt(string base64Input, string key)
        {
            byte[] encryptBytes = Convert.FromBase64String(base64Input);

            byte[] decryptBytes = Decrypt(encryptBytes, key);

            string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);

            return decryptedString;
        }

    }
}