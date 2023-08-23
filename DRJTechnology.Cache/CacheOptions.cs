    using System;
    using Microsoft.AspNetCore.Http;

namespace DRJTechnology.Cache
{
    public class CacheOptions
    {
        public string RedisConnectionString { get; set; } = string.Empty;

        public string KeyPrefix { get; set; } = string.Empty;

        public int DefaultExpiryInMinutes { get; set; } = 60;
    }
}