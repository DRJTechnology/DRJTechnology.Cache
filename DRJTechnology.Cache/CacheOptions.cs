namespace DRJTechnology.Cache
{
    public class CacheOptions
    {
        public bool Enabled { get; set; } = false;

        public string ConnectionString { get; set; } = string.Empty;

        public string KeyPrefix { get; set; } = string.Empty;

        public int DefaultExpiryInMinutes { get; set; } = 60;
    }
}