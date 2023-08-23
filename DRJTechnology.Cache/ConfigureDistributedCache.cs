using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DRJTechnology.Cache
{
    public static class ConfigureDistributedCache
    {
        public static void AddDistributedCache(this IServiceCollection services, Action<CacheOptions> optionsBuilder)
        {
            var options = new CacheOptions();
            optionsBuilder?.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.RedisConnectionString))
            {
                throw new Exception("Redis connection string missing from configuration file.");
            }

            services.AddSingleton(options);
            services.AddStackExchangeRedisCache(option => { option.Configuration = options.RedisConnectionString; });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}