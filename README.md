# DRJTechnology.Cache
Component to interface with Redis cache

## dotnetcore implementation
Written in C#, this package provides an interface to redis cache from dotnetcore applications

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

