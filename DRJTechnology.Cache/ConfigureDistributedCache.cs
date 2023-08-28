using Microsoft.Extensions.DependencyInjection;

namespace DRJTechnology.Cache
{
    public static class ConfigureDistributedCache
    {
        public static void AddDistributedCache(this IServiceCollection services, Action<CacheOptions> optionsBuilder)
        {
            var options = new CacheOptions();
            optionsBuilder?.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new Exception("Cache connection string missing from configuration file.");
            }

            services.AddSingleton(options);
            services.AddStackExchangeRedisCache(option => { option.Configuration = options.ConnectionString; });
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}