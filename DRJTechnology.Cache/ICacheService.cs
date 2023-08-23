namespace DRJTechnology.Cache
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);

        Task<(bool Success, T Value)> TryGetAsync<T>(string key);

        Task<T> SetAsync<T>(string key, T value);

        Task<T> SetAsync<T>(string key, T value, int? expiryInMinutes, bool slidingExpiration = true);

        Task RemoveAsync(string key);

        Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue);

        Task<T> GetOrCreateAsync<T>(string key, Func<T> getValue, int? expiryInMinutes, bool slidingExpiration = true);

        Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue);

        Task<T> GetOrFetchAsync<T>(string key, Func<Task<T>> getValue, int? expiryInMinutes, bool slidingExpiration = true);
    }
}
