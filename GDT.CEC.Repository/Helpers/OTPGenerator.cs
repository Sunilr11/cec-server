using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository
{
    public class OTPGenerator
    {
        public static string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999); 
            return otp.ToString("D6"); 
        }

        public static  string HashOTP(string otp)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private static readonly Random random = new Random();
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string Symbols = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

        public static string GeneratePassword(int length = 8)
        {
           
            StringBuilder password = new StringBuilder();
            password.Append(GetRandomCharacter(Lowercase));
            password.Append(GetRandomCharacter(Numbers));
            password.Append(GetRandomCharacter(Symbols));

            string allCharacters = Lowercase;
            for (int i = 3; i < length; i++)
            {
                password.Append(GetRandomCharacter(allCharacters));
            }

            return ShufflePassword(password.ToString());
        }

        private static char GetRandomCharacter(string characters)
        {
            int index = random.Next(characters.Length);
            return characters[index];
        }

        private static string ShufflePassword(string password)
        {
            char[] array = password.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

    }
}
