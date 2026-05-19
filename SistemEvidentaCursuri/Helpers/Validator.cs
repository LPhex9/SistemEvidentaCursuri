using System.Text.RegularExpressions;

namespace SistemEvidentaCursuri.Helpers
{
    public static class Validator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone.Trim(), @"^(\+373|0)\d{8}$");
        }
    }
}
