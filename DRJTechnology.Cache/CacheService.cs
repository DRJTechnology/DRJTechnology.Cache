using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DRJTechnology.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly CacheOptions options;
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        public CacheService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor, CacheOptions cacheOptions)
        {
            this.cache = cache;
            this.httpContextAccessor = httpContextAccessor;
            this.options = cacheOptions;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cacheKey = this.GetKey(key);
            var value = await this.cache.GetStringAsync(cacheKey);
            if (value == null)
            {
                var retVal = default(T);
                return retVal == null ? retVal : throw new CacheNotFoundException(cacheKey);
            }

            return this.Deserialize<T>(value);
        }

        public async Task<(bool Success, T Value)> TryGetAsync<T>(string key)
        {
            var cacheValue = await this.cache.GetStringAsync(this.GetKey(key));
            if (cacheValue != null)
            {
                return (true, this.Deserialize<T>(cacheValue));
            }

            return (false, default);
        }

        public async Task<T> SetAsync<T>(string key, T value)
        {
            return await this.SetAsync(key, value, this.options.DefaultExpiryInMinutes);
        }

        public async Task<T> SetAsync<T>(string key, T value, int? expiryInMinutes, bool slidingExpiration = false)
        {
            var cacheValue = typeof(T) == typeof(string) ? value as string
                : JsonSerializer.Serialize(value, this.jsonSerializerOptions);

            if (expiryInMinutes.HasValue)
            {
                var expiryTime = new TimeSpan(0, expiryInMinutes.Value, 0);
                var options = new DistributedCacheEntryOptions();

                if (slidingExpiration)
                {
                    options.SlidingExpiration = expiryTime;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = expiryTime;
                }

                await this.cache.SetStringAsync(this.GetKey(key), cacheValue, options);
            }
            else
            {
                await this.cache.SetStringAsync(this.GetKey(key), cacheValue);
            }

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await this.cache.RemoveAsync(this.GetKey(key));
        }

        /// <inheritdoc />
        public async Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue)
        {
            return await this.GetOrCreateAsync(key, getValue, this.options.DefaultExpiryInMinutes);
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue, int? expiryInMinutes, bool slidingExpiration = false)
        {
            var value = await this.cache.GetStringAsync(this.GetKey(key));
            if (value != null)
            {
                return this.Deserialize<T>(value);
            }

            return await this.SetAsync(key, getValue(), expiryInMinutes, slidingExpiration);
        }

        public async Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue)
        {
            return await this.GetOrFetchAsync(key, getValue, this.options.DefaultExpiryInMinutes);
        }

        public async Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue, int? expiryInMinutes, bool slidingExpiration = false)
        {
            var value = await this.cache.GetStringAsync(this.GetKey(key));
            if (value != null)
            {
                return this.Deserialize<T>(value);
            }

            return await this.SetAsync(key, await getValue(), expiryInMinutes, slidingExpiration);
        }

        private T Deserialize<T>(string value)
        {
            return typeof(T) == typeof(string) ? (T)(object)value : JsonSerializer.Deserialize<T>(value, this.jsonSerializerOptions);
        }

        private string GetKey(string key)
        {
            var keyValue = string.Empty;

            if (!string.IsNullOrWhiteSpace(this.options.KeyPrefix))
            {
                keyValue += $"{this.options.KeyPrefix}:";
            }

            keyValue += key;

            return keyValue;
        }
    }
}