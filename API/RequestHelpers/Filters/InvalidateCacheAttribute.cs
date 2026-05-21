using Microsoft.AspNetCore.Mvc;

namespace API.RequestHelpers.Filters
{
    public class InvalidateCacheAttribute : TypeFilterAttribute
    {
        public InvalidateCacheAttribute(string pattern) : base(typeof(InvalidateCacheFilter))
        {
            Arguments = [pattern];
        }
    }
}
