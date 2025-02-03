using System;
using System.Security.Cryptography;

namespace HotelBooking_API.Data.Other
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // Размер соли в байтах
        private const int HashSize = 20; // Размер хеша в байтах
        private const int Iterations = 10000; // Количество итераций

        public static byte[] HashPassword(string password)
        {
            // Генерация соли
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Хеширование пароля с использованием PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // Объединение соли и хеша
                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                return hashBytes;
            }
        }
        public static bool VerifyPassword(string password, byte[] storedHash)
        {
            byte[] salt = new byte[SaltSize];
            Array.Copy(storedHash, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                for (int i = 0; i < HashSize; i++)
                {
                    if (hash[i] != storedHash[i + SaltSize]) return false;
                }
            }
            return true;
        }

    }
}
