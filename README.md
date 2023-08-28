# DRJTechnology.Cache
Component to interface with Redis cache

## dotnetcore implementation
Written in C#, this package provides an interface to redis cache from dotnetcore applications

Available on nuget.org at: https://www.nuget.org/packages/DRJTechnology.Cache/1.0.0

### Configuration
The following parameters are picked up from the calling application
 
    "Enabled": false,  
    "KeyPrefix": "",  
    "DefaultExpiryInMinutes":  60,
    "ConnectionString": "",

    Enabled:                Enabled or disables the retrieval & storage of cached inforation. If no cache is currently available tbis can be set to false to prevent errors beinf thrown.
    KeyPrefix:              All storage keys are prefixed with this value.
    DefaultExpiryInMinutes: If no expiry value is set on the individual call, this value is used.
    ConnectionString:       The redis connection string.

### Example use:  

Code required in the Program.cs file before the builder.Build()  

    // Set up caching.
    var keyPrefix = builder.Configuration.GetValue<string>("DRJCache:KeyPrefix");
    builder.Services.AddDistributedCache(opt =>
    {
        opt.Enabled = builder.Configuration.GetValue<bool>("DRJCache:Enabled");
        opt.ConnectionString = builder.Configuration.GetValue<string>("DRJCache:ConnectionString") ?? string.Empty;
        opt.KeyPrefix = $"{keyPrefix}_WebUI_";
        opt.DefaultExpiryInMinutes = builder.Configuration.GetValue<int>("DRJCache:DefaultExpiryInMinutes");
    });

    var app = builder.Build();

Calles to add/retrieve values from cache.

    ICacheService cacheService  
    
    var key = "ExampleKey";
    var cacheObj = new ExampleObject { Id = 99, Name="Mr X", AnotherProperty = "Some value." };

    // Set value in cache using the default cache expiry time
    await cacheService.SetAsync(key, cacheObj);

    // Try Get value from cache by key
    var (success, returnedCacheObj) = cacheService.TryGetAsync<ExampleObject>(key).Result;
    if (success) ....

    // Get value from cache by key
    var fromCacheObj1 = cacheService.GetAsync<ExampleObject>(key).Result;

    // Set value in cache expiry time of 45 mins
    await cacheService.SetAsync(key, cacheObj, 45);

    // Set value in cache with expiry time of 15 mins as sliding expiration
    await cacheService.SetAsync(key, cacheObj, 15, true);

    // Remove value from cache by key
    await cacheService.RemoveAsync(key);

    // Get or create value in from/in cache if it doesn't exists
    await cacheService.GetOrCreateAsync(key, () => cacheObj);

