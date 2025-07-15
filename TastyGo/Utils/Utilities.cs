using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TastyGo.Utils;

public class Utilities
{
    public static bool IsStringNumericRegex(string input)
    {
        return Regex.IsMatch(input, @"^\d+$");
    }

    public static bool IsStringNumericLinq(string input)
    {
        return input.All(char.IsDigit);
    }

    public static bool IsStringNumericManual(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    public static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

}
