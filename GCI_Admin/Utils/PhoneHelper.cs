using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class PhoneHelper
    {
        public static string NormalizeKenyanPhoneOrEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Replace(" ", "").Trim();

            if (!input.All(c => char.IsDigit(c) || c == '+'))
                return input;

            if ((input.StartsWith("+2547") || input.StartsWith("+2541")) && input.Length == 13)
            {
                return input;
            }
            else if ((input.StartsWith("2547") || input.StartsWith("2541")) && input.Length == 12)
            {
                return "+" + input;
            }
            else if ((input.StartsWith("07") || input.StartsWith("01")) && input.Length == 10)
            {
                return "+254" + input.Substring(1);
            }
            else if ((input.StartsWith("7") || input.StartsWith("1")) && input.Length == 9)
            {
                return "+254" + input;
            }

            return input;
        }

        public static string GenerateTempPhone(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return $"0700{DateTime.Now.Ticks.ToString().Substring(10)}_TEMP";
                }

                var hash = email.GetHashCode();

                if (hash < 0) hash = Math.Abs(hash);

                var uniquePart = (hash % 10000000).ToString().PadLeft(7, '0');

                var tempPhone = $"070{uniquePart.Substring(0, 7)}_TEMP";

                return tempPhone;
            }
            catch
            {
                return $"0700{DateTime.Now.Ticks.ToString().Substring(8)}_TEMP";
            }
        }
    }
}
