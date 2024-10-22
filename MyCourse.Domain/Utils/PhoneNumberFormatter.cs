using System.Text.RegularExpressions;

namespace MyCourse.Domain.Utils
{
    public static class PhoneNumberExtensions
    {
        public static string FormatAustrianPhoneNumber(this string phoneNumber)
        {
            phoneNumber = Regex.Replace(phoneNumber, @"\D", "");

            phoneNumber = phoneNumber switch
            {
                var number when number.StartsWith("43") => $"+{number}",
                var number when number.StartsWith("0043") => $"+{number.Substring(2)}",
                var number when number.StartsWith("0") => $"+43{number.Substring(1)}",
                _ => $"+43{phoneNumber}",
            };

            return phoneNumber;
        }
    }
}
