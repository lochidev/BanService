using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace BanService.Extensions
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data,
            TimeSpan? absoluteExpirationTime = null, TimeSpan? unusedExpirationTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationTime ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = unusedExpirationTime
            };
            var jsonData = JsonSerializer.Serialize(data, JsonSerializerOptions);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            if (recordId is null) return default;
            if (recordId.Contains("BanService_")) recordId = recordId.Replace("BanService_", string.Empty);
            var jsonData = await cache.GetStringAsync(recordId);
            if (jsonData is null) return default;
            return JsonSerializer.Deserialize<T>(jsonData, JsonSerializerOptions);
        }
    }
}