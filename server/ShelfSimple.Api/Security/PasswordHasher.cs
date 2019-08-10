using System;
using System.Security.Cryptography;

namespace ShelfSimple.Api.Security
{
    // PBKDF2 (SHA256). Stored as "v1.iterations.salt.hash" so we can bump
    // the iteration count later without breaking old hashes.
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Derive(password, salt, Iterations);
            return string.Join(".", "v1", Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;

            var parts = stored.Split('.');
            if (parts.Length != 4 || parts[0] != "v1")
                return false;

            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Derive(password, salt, iterations);

            return FixedTimeEquals(expected, actual);
        }

        private static byte[] Derive(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
