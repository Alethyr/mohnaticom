using Core.Intrefaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers.Filters
{
    public class InvalidateCacheFilter : IAsyncActionFilter
    {
        private readonly string _pattern;
        private readonly IResponseCacheService _cacheService;
        public InvalidateCacheFilter(string pattern, IResponseCacheService cacheService)
        {
            _pattern = pattern;
            _cacheService = cacheService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (resultContext.Exception is null || resultContext.ExceptionHandled)
            {
                await _cacheService.RemoveCacheByPattern(_pattern);
            }
        }
    }
}
