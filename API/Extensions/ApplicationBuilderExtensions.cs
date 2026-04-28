using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationBuilderExtensions
{
   public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<StoreContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            await context.Database.MigrateAsync();
            await StoreContextSeed.SeedAsync(context, userManager);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
