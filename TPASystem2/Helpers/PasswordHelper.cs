// =============================================
// PasswordHelper.cs
// Utility class for password hashing operations - EXACT match for TPASystem2
// Compatible with C# 7.3 and earlier versions
// Place this in your Helpers folder or appropriate namespace
// =============================================

using System;
using System.Security.Cryptography;
using System.Text;

namespace TPASystem2.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Generates a random salt for password hashing (EXACT match for TPASystem2)
        /// </summary>
        /// <returns>A unique salt string in Base64 format</returns>
        public static string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        /// <summary>
        /// Hashes a password with the provided salt (EXACT match for TPASystem2)
        /// Algorithm: SHA256(password + salt) -> Base64
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt to use</param>
        /// <returns>The hashed password in Base64 format</returns>
        public static string ComputeHash(string password, string salt)
        {
            // EXACT same algorithm as TPASystem2
            var combined = password + salt;
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifies a password against a hash and salt (EXACT match for TPASystem2)
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="storedHash">The stored hash</param>
        /// <param name="salt">The stored salt</param>
        /// <returns>True if password matches</returns>
        public static bool VerifyPassword(string password, string salt, string storedHash)
        {
            try
            {
                // Use SHA256 with salt (matching the TPASystem2 implementation)
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    string saltedPassword = password + salt;
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                    string computedHash = Convert.ToBase64String(bytes);
                    return computedHash == storedHash;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a temporary password
        /// </summary>
        /// <param name="length">Length of the password (default 12)</param>
        /// <returns>A random temporary password</returns>
        public static string GenerateTemporaryPassword(int length = 12)
        {
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}