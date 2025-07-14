using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Web
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
    }
}
