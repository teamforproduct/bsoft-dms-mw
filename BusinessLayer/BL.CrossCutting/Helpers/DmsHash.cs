using System;
using System.Security.Cryptography;
using System.Text;

namespace BL.CrossCutting.Helpers
{
    public static class DmsHash
    {
        public static string GetSha512(string input)
        {
            byte[] buffer = Encoding.Default.GetBytes(input);
            var sha = new SHA512Managed();
            byte[] hash = sha.ComputeHash(buffer);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        public static bool VerifySha512(string input, string hash)
        {
            string hashOfInput = GetSha512(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        public static bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}