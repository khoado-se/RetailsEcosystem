using System.Text;
using System.Text.Json;

namespace RetailsEcosystem.Customer.Web.Helpers;

public static class JwtHelper
{
    public static DateTime? GetTokenExpiry(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            var padded = (payload.Length % 4) switch
            {
                2 => payload + "==",
                3 => payload + "=",
                _ => payload
            };
            padded = padded.Replace('-', '+').Replace('_', '/');
            var bytes = Convert.FromBase64String(padded);
            using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(bytes));

            if (!doc.RootElement.TryGetProperty("exp", out var expEl)) return null;
            return DateTimeOffset.FromUnixTimeSeconds(expEl.GetInt64()).UtcDateTime;
        }
        catch
        {
            return null;
        }
    }
}
