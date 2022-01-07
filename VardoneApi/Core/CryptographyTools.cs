using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VardoneApi.Config;

namespace VardoneApi.Core
{
    internal static class CryptographyTools
    {
        public static string GetSha512Hash(byte[] bytes)
        {
            var sb = new StringBuilder();
            using (var sha512 = SHA512.Create())
            {
                var computeHash = sha512.ComputeHash(bytes);
                foreach (var b in computeHash) sb.Append(b.ToString("X"));
            }
            return sb.ToString();
        }

        public static byte[] GetByteKey(string key, string salt)
        {
            return new Rfc2898DeriveBytes(GetSha512Hash(Encoding.ASCII.GetBytes(key)),
                Encoding.ASCII.GetBytes(salt),
                7).GetBytes(16);
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
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

        public static string GetPasswordHash(string pus, string passwordHash)
        {
            for (var i = 0; i < Math.Pow(2, 16); i++)
            {
                passwordHash = i switch
                {
                    0 => GetSha512Hash(Encoding.ASCII.GetBytes(passwordHash + pus)),
                    1 => GetSha512Hash(Encoding.ASCII.GetBytes(passwordHash + PasswordOptions.KEY)),
                    _ => GetSha512Hash(Encoding.ASCII.GetBytes(passwordHash))
                };
            }
            return passwordHash;
        }
    }
}