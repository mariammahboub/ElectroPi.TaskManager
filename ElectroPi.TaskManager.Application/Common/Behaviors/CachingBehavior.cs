using ElectroPi.TaskManager.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Behaviors
{


    public sealed class CachingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ICacheService _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(
            ICacheService cache,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheableQuery)
                return await next();

            if (cacheableQuery.BypassCache)
            {
                _logger.LogDebug("[Cache] Bypass requested for key: {Key}", cacheableQuery.CacheKey);
                return await next();
            }

            var cached = await _cache.GetAsync<TResponse>(
                cacheableQuery.CacheKey, cancellationToken);

            if (cached is not null)
            {
                _logger.LogDebug("[Cache] HIT — {Key}", cacheableQuery.CacheKey);
                return cached;
            }

            _logger.LogDebug("[Cache] MISS — {Key}", cacheableQuery.CacheKey);
            var response = await next();

            await _cache.SetAsync(
                cacheableQuery.CacheKey,
                response,
                cacheableQuery.CacheExpiry,
                cancellationToken);

            return response;
        }
    }
    }
