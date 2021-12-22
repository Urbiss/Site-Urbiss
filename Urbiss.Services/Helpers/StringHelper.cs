using System;
using System.Security.Cryptography;
using System.Text;

namespace Urbiss.Services.Helpers
{
    public static class StringHelper
    {
        public static string GenerateRandomString(int size)
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
                result.Append(chars[b % (chars.Length)]);
            return result.ToString();
        }

        public static string ToBase64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string FromBase64(string text)
        {
            var base64EncodedBytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
