using System.Text.Json.Serialization;

namespace MiniATM.Infrastructure.Models
{
    public class CacheOptions
    {
        public CacheTypes Type { get; set; } = CacheTypes.Memory;
        public CacheSqlServerOptions? SqlServerOptions { get; set; }
        public CacheRedisOptions? RedisOptions { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter<CacheTypes>))]
    public enum CacheTypes
    {
        Memory,
        SqlServer,
        Redis
    }

    public class CacheSqlServerOptions
    {
        public string ConnectionStringName { get; set; } = "SqlServerCacheConnection";
        public string SchemaName { get; set; } = "dbo";
        public string TableName { get; set; } = "_Cache";
    }

    public class CacheRedisOptions
    {
        public string ConnectionStringName { get; set; } = "RedisCacheConnection";
    }
}
