using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BanService.Services
{
    public class BannerService : Banner.BannerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<BannerService> _logger;

        public BannerService(IDistributedCache distributedCache, ILogger<BannerService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public override async Task<BanResponse> BanUser(BanRequest request, ServerCallContext context)
        {
            try
            {
                DistributedCacheEntryOptions cacheEntryOptions = new()
                {
                    AbsoluteExpirationRelativeToNow = request.Duration.ToTimeSpan()
                };
                await _distributedCache.SetStringAsync($"{request.Identifier}_{request.ServiceId}", request.ServiceId,
                    cacheEntryOptions);
                return new BanResponse {IsBanned = true};
            }
            catch (Exception e)
            {
                _logger.LogCritical("Exception when banning user: {Message}", e.Message);
                return new BanResponse {IsBanned = false};
            }
        }

        public override async Task<IsBannedResponse> IsBanned(IsBannedRequest request, ServerCallContext context)
        {
            var data = await _distributedCache.GetStringAsync($"{request.Identifier}_{request.ServiceId}");
            return new IsBannedResponse
            {
                IsBanned = data != null
            };
        }

        public override async Task<UnbanResponse> UnbanUser(UnbanRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Identifier) || string.IsNullOrEmpty(request.ServiceId))
                return new UnbanResponse {Success = false};
            await _distributedCache.RemoveAsync($"{request.Identifier}_{request.ServiceId}");
            return new UnbanResponse {Success = true};
        }
    }
}