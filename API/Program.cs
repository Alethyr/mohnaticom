using API.Middleware;
using Core.Entities;
using API.SignalR;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();
app.MapHub<NotificationHub>("hub/notifications");

await app.ApplyMigrationsAndSeedAsync();

app.Run();
