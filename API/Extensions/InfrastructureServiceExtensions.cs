using Core.Intrefaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stripe;

namespace API.Extensions;

public static class InfrastructureServiceExtensions
{       
   public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddDbContext<StoreContext>(opt =>
        {   
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IConnectionMultiplexer>( _ =>
        {
            var connString = config.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis not configured");
            var options = ConfigurationOptions.Parse(connString, true);

            options.AbortOnConnectFail = false;
            options.ConnectRetry = 3;

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        services.AddSingleton<ICartService, CartService>();
        services.AddSingleton(_ => 
            new StripeClient(config["StripeSettings:SecretKey"]));

        services.AddScoped<ICouponService, Infrastructure.Services.CouponService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
