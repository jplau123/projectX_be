using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace project_backend.Attributes
{
    public class PasswordRequirementsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            string? password = value as string;

            if (password == null) return false;

            // Regexes to check for
            Regex uppercaseRegex = new Regex(@"[A-Z]");
            Regex numberRegex = new Regex(@"[0-9]");

            bool hasUppercase = uppercaseRegex.IsMatch(password);
            bool hasNumber = numberRegex.IsMatch(password);

            return hasUppercase && hasNumber;
        }
    }
}
