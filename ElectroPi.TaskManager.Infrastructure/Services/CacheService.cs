using ElectroPi.TaskManager.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Services
{

    public sealed class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<CacheService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public CacheService(
            IDistributedCache cache,
            IConnectionMultiplexer redis,
            ILogger<CacheService> logger)
        {
            _cache = cache;
            _redis = redis;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(
      string key,
      CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = await _cache.GetAsync(key, cancellationToken);
                if (bytes is null || bytes.Length == 0) return default;
                return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] GET failed for key '{Key}'. Running without cache.", key);
                return default;   
            }
        }
        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);

                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                    options.SetAbsoluteExpiration(expiry.Value);
                else
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                await _cache.SetAsync(key, bytes, options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] SET failed for key '{Key}'. Continuing without cache.", key);
            }
        }
        public async Task RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] REMOVE failed for key '{Key}'.", key);
            }
        }

        public async Task RemoveByPrefixAsync(
            string prefix,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var database = _redis.GetDatabase();
                var pattern = $"{prefix}*";

                await foreach (var key in server.KeysAsync(pattern: pattern))
                {
                    await database.KeyDeleteAsync(key);
                    _logger.LogDebug("[Cache] Deleted key '{Key}' via prefix scan.", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] PREFIX REMOVE failed for prefix '{Prefix}'.", prefix);
            }
        }

        public async Task<bool> ExistsAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var bytes = await _cache.GetAsync(key, cancellationToken);
                return bytes is not null && bytes.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] EXISTS check failed for key '{Key}'.", key);
                return false;
            }
        }
    }
}