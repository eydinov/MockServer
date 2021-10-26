using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ServerHost
{
    public static class Server
    {

        public static void Run(IConfiguration configuration, params string[] urls)
        {
            CreateWebHostBuilder(urls, configuration).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] urls, IConfiguration configuration)
        {
            return WebHost.CreateDefaultBuilder()
              .UseUrls(urls)
              .ConfigureAppConfiguration((context, config) =>
              {
                  config.AddConfiguration(configuration);
              })
             .UseConfiguration(configuration)
             .UseStartup<Startup>();
        }

    }
}
