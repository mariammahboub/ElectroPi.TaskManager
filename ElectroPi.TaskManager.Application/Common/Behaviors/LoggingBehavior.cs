using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Behaviors
{

    public sealed class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private const int SlowRequestThresholdMs = 500;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "[ElectroPi] Handling {RequestName} | {@Request}",
                requestName, request);

            try
            {
                var response = await next();
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
                {
                    _logger.LogWarning(
                        "[ElectroPi] SLOW REQUEST — {RequestName} took {Elapsed}ms",
                        requestName, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation(
                        "[ElectroPi] Handled {RequestName} in {Elapsed}ms",
                        requestName, stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "[ElectroPi] Error handling {RequestName} after {Elapsed}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}