using System;
using System.Security.Cryptography;

namespace Petlance
{
    public static class SecurePasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public static string Hash(string password, int iterations)
        {
            byte[] salt, hash;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                hash = pbkdf2.GetBytes(HashSize);
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
            return string.Format("{0}${1}", iterations, Convert.ToBase64String(hashBytes));
        }
        public static string Hash(string password) => Hash(password, 10000);
        public static bool Verify(string password, string hashedPassword)
        {
            string[] splittedHashString = hashedPassword.Split('$');
            byte[] hashBytes = Convert.FromBase64String(splittedHashString[1]);
            byte[] salt = new byte[SaltSize], hash;
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, int.Parse(splittedHashString[0])))
                hash = pbkdf2.GetBytes(HashSize);
            for (var i = 0; i < HashSize; i++)
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            return true;
        }
    }
}