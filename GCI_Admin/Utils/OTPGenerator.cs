using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{

    public static class OTPGenerator
    {
        private static readonly Random _random = new Random();

        public static string GenerateOTP()
        {
            const string digits = "0123456789";
            var otp = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                int index = _random.Next(digits.Length);
                otp.Append(digits[index]);
            }

            return otp.ToString();
        }
       

        public static  bool IsPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            var phonePattern = @"^(?:\+254|0)?7\d{8}$";
            return Regex.IsMatch(input, phonePattern);
        }

        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // Simple email validation
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(input, emailPattern, RegexOptions.IgnoreCase);
        }

    }
}
