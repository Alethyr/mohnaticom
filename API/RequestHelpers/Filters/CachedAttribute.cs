using Microsoft.AspNetCore.Mvc;

namespace API.RequestHelpers.Filters
{
    public class CachedAttribute : TypeFilterAttribute
    {
        public CachedAttribute(int timeToLiveSeconds) : base(typeof(CachedFilter)) 
        {
            Arguments = [timeToLiveSeconds];
        }
    }
}
