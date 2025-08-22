using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;

namespace CEGA.Services
{
    public class OtpService
    {
        private readonly IDistributedCache _cache;
        public OtpService(IDistributedCache cache) => _cache = cache;

        private static string Key(string userId) => $"pwd:otp:{userId}";
        private static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));

        public async Task<string> GenerateAndStoreAsync(string userId, int minutes = 10)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            await _cache.SetStringAsync(Key(userId), Hash(code),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes) });
            await _cache.SetStringAsync(Key(userId) + ":tries", "0",
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes) });
            return code;
        }

        public async Task<bool> ValidateAsync(string userId, string input, int maxTries = 5)
        {
            var key = Key(userId);
            var stored = await _cache.GetStringAsync(key);
            if (stored is null) return false;

            var triesKey = key + ":tries";
            var tries = int.Parse(await _cache.GetStringAsync(triesKey) ?? "0");
            if (tries >= maxTries) return false;

            var ok = CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(stored), Convert.FromHexString(Hash(input)));

            await _cache.SetStringAsync(triesKey, (tries + 1).ToString());
            if (ok)
            {
                await _cache.RemoveAsync(key);
                await _cache.RemoveAsync(triesKey);
            }
            return ok;
        }
    }
}
