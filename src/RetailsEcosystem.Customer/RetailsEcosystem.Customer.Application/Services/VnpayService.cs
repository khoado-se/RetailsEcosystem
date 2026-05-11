using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Settings;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly VnpaySettings _settings;
        private readonly HttpClient _httpClient;

        public VnpayService(IOptions<VnpaySettings> options, IHttpClientFactory factory)
        {
            _settings   = options.Value;
            _httpClient = factory.CreateClient("VNPay");
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

        public async Task<VnpayQueryResult> QueryTransactionAsync(string txnRef, string transactionDate, string ipAddress)
        {
            var now       = GetVietnamTime();
            var requestId = Guid.NewGuid().ToString("N");

            var rawData = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_RequestId"]       = requestId,
                ["vnp_Version"]         = "2.1.0",
                ["vnp_Command"]         = "querydr",
                ["vnp_TmnCode"]         = _settings.TmnCode,
                ["vnp_TxnRef"]          = txnRef,
                ["vnp_OrderInfo"]       = $"Query transaction {txnRef}",
                ["vnp_TransactionDate"] = transactionDate,
                ["vnp_CreateDate"]      = now.ToString("yyyyMMddHHmmss"),
                ["vnp_IpAddr"]          = ipAddress,
            };

            var hashData = string.Join("|", rawData.Values);
            rawData["vnp_SecureHash"] = ComputeHmacSha512(_settings.HashSecret, hashData);

            var json     = JsonSerializer.Serialize(rawData);
            var response = await _httpClient.PostAsync(
                _settings.QueryDrUrl,
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                return new VnpayQueryResult { ResponseCode = "99", Message = "Query failed" };

            var content = await response.Content.ReadAsStringAsync();
            using var doc  = JsonDocument.Parse(content);
            var root = doc.RootElement;

            return new VnpayQueryResult
            {
                ResponseCode      = root.TryGetProperty("vnp_ResponseCode",      out var rc)  ? rc.GetString()  ?? "" : "",
                TransactionStatus = root.TryGetProperty("vnp_TransactionStatus", out var ts)  ? ts.GetString()  ?? "" : "",
                TransactionNo     = root.TryGetProperty("vnp_TransactionNo",     out var tn)  ? tn.GetString()  ?? "" : "",
                Amount            = root.TryGetProperty("vnp_Amount",            out var amt) && long.TryParse(amt.GetString(), out var a) ? a : 0,
                Message           = root.TryGetProperty("vnp_Message",           out var msg) ? msg.GetString() ?? "" : "",
            };
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
