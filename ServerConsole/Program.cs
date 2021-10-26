using Microsoft.Extensions.Configuration;
using ServerHost;
using System;
using System.IO;

namespace MockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration");

            foreach (var file in Directory.GetFiles(path, "*.json"))
            {
                config.AddJsonFile(file, optional: false, reloadOnChange: true);
            }

            var urls = System.Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';');

            Server.Run(config.Build(), urls);
        }
    }
}
