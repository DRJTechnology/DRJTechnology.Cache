using System;

namespace DRJTechnology.Cache
{
    public class CacheNotFoundException : Exception
    {
        public CacheNotFoundException(string key)
            : base($"No value found in cache for key: {key}")
        {
        }
    }
}