using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace GDT.CEC.Web.Helpers
{
    public class Common
    {
       
        static string emailPattern = @"^([^\s@]+@[^\s@]+\.[^\s@]+)$";

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        public static IFormFile Base64ToFormFile(string base64String, string fileName)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            using MemoryStream ms = new MemoryStream(bytes);
            return new FormFile(ms, 0, bytes.Length, "file", fileName);
        }
    }
}
