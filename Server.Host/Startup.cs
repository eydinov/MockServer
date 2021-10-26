using MockServer.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerHost.Middlewares;
using ServerHost.Services;
using MockServer.Environment.Abstractions;

namespace ServerHost
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MockConfiguration>(configuration.GetSection("RequestCollection"));

            services.AddTransient<IMatcher, MatcherService>();
            services.AddSingleton<IAuthorizationService, AuthService>();
            services.AddSingleton<IBasicAuthorizationHandler, BasicAuthHandler>();
            services.AddSingleton<IBearerAuthorizationHandler, BearerAuthHandler>();
            services.AddSingleton<IApiKeyAuthorizationHandler, ApiKeyAuthHandler>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<MockExecutor>();
            app.UseStaticFiles();
        }

    }
}
