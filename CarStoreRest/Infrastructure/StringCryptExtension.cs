using System;
using System.Security.Cryptography;
using System.Text;

namespace CarStoreRest.Infrastructure
{
    public static class StringCryptExtension // in rest part for test!!!
    {
        
        private const string key = "key12345";
        private const string iv = "ivi54321";
        private static byte[] dataKey = Encoding.ASCII.GetBytes(key);
        private static byte[] dataIV = Encoding.ASCII.GetBytes(iv);


        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(dataKey, dataIV);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(dataKey, dataIV);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }

}
