using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core
{
    public struct PasswordInformation
    {
        private byte[] hash;
        private string salt;

        public PasswordInformation(byte[] hash, string salt)
        {
            this.hash = hash;
            this.salt = salt;
        }

        private static byte[] SaltAndHash(string password, string salt)
        {
            byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(saltedPassword);
            }
        }

        public bool Match(string password)
        {
            return Enumerable.SequenceEqual(this.hash, SaltAndHash(password, this.salt));
        }

        public static PasswordInformation FromPassword(string password)
        {
            string salt = new RandomDataGenerator().AlphaNumericString(32);
            return new PasswordInformation(SaltAndHash(password, salt), salt);
        }
    }
}
