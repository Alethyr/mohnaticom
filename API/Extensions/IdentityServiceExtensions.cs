using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddAuthorization();

        services.AddIdentityApiEndpoints<AppUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StoreContext>();

        return services;
    }
}
