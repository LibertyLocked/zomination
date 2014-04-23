using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DRM
{
    public static class Keygen
    {
        /// <summary>
        /// Encrypt a string using a rgbIV and a key.
        /// </summary>
        /// <param name="ClearText">String to encrypt</param>
        /// <param name="srgbIV">String of rgbIV</param>
        /// <param name="skey">String of key</param>
        /// <returns>Returns the encrypted string</returns>
        public static string Encrypt(string ClearText, string srgbIV, string skey)
        {
            byte[] clearTextBytes = Encoding.UTF8.GetBytes(ClearText);

            System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();

            MemoryStream ms = new MemoryStream();
            byte[] rgbIV = Encoding.ASCII.GetBytes(srgbIV);
            byte[] key = Encoding.ASCII.GetBytes(skey);
            CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);

            cs.Write(clearTextBytes, 0, clearTextBytes.Length);

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypt a string using a rgbIV and a key.
        /// </summary>
        /// <param name="EncryptedText">String to decrypt</param>
        /// <param name="srgbIV">String of rgbIV</param>
        /// <param name="skey">String of key</param>
        /// <returns>Returns the decrypted string</returns>
        public static string Decrypt(string EncryptedText, string srgbIV, string skey)
        {
            byte[] encryptedTextBytes = Convert.FromBase64String(EncryptedText);

            MemoryStream ms = new MemoryStream();

            System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();


            byte[] rgbIV = Encoding.ASCII.GetBytes(srgbIV);
            byte[] key = Encoding.ASCII.GetBytes(skey);

            CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);

            cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);

            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());

        }

        /// <summary>
        /// Calculate MD5 Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
