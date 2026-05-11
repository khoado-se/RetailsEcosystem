using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Settings;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class VnpayWebService : IVnpayWebService
    {
        private readonly VnpaySettings _settings;

        public VnpayWebService(IOptions<VnpaySettings> options)
        {
            _settings = options.Value;
        }

        public string BuildPaymentUrl(OrderDto order, string txnRef, string ipAddress)
        {
            var now = GetVietnamTime();

            var rawData = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"]    = "2.1.0",
                ["vnp_Command"]    = "pay",
                ["vnp_TmnCode"]    = _settings.TmnCode,
                ["vnp_Amount"]     = ((long)(order.TotalAmount * 100)).ToString(),
                ["vnp_CurrCode"]   = "VND",
                ["vnp_TxnRef"]     = txnRef,
                ["vnp_OrderInfo"]  = $"Thanh toan don hang {order.Id}",
                ["vnp_OrderType"]  = "other",
                ["vnp_Locale"]     = "vn",
                ["vnp_ReturnUrl"]  = _settings.ReturnUrl,
                ["vnp_IpAddr"]     = ipAddress,
                ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
                ["vnp_ExpireDate"] = now.AddMinutes(15).ToString("yyyyMMddHHmmss"),
            };

            var hashData   = string.Join("&", rawData.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
            var secureHash = ComputeHmacSha512(_settings.HashSecret, hashData);
            rawData["vnp_SecureHash"] = secureHash;

            var query = string.Join("&", rawData.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            return $"{_settings.PaymentUrl}?{query}";
        }

        public bool ValidateSignature(IDictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("vnp_SecureHash", out var receivedHash))
                return false;

            var rawData = string.Join("&",
                parameters
                    .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                    .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                    .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var computed = ComputeHmacSha512(_settings.HashSecret, rawData);
            return string.Equals(computed, receivedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static DateTime GetVietnamTime()
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
            }
        }

        private static string ComputeHmacSha512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
        }
    }
}
