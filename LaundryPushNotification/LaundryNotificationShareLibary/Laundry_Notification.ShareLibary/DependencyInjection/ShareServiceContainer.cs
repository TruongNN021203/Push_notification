using Laundry_Notification.ShareLibary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace Laundry_Notification.ShareLibary.DependencyInjection
{

    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName, bool enableDbContext = true) where TContext : DbContext
        {
            //add generic database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config
                .GetConnectionString("NotificationConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));


            //configure serilog loggin
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}]{message:lj}{NewLine}{Exception}",
              rollingInterval: RollingInterval.Day)
                                .CreateLogger();


            return services;
        }

        public static IApplicationBuilder UseSharesPolicies(this IApplicationBuilder app)
        {
            //use global exception
            app.UseMiddleware<GlobalException>();


            return app;
        }
    }
}

