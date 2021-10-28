using MockServer.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerHost.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using MockServer.Environment.Extensions;
using MockServer.Environment.Abstractions;

namespace ServerHost.Services
{
    public class AssemblyMatcher : ResponseMatcher
    {
        const string ASSEMBLY_NAME_PROPS = "Assembly";
        const string ASSEMBLY_CLASS_PROPS = "Class";

        readonly IMockPlugin _plugin;
        readonly IServiceProvider _provider;
        readonly ILogger<AssemblyMatcher> _logger;

        public AssemblyMatcher(IServiceProvider provider, ILogger<AssemblyMatcher> logger, MockOption option, IAuthorizationService authService) : base(option, authService)
        {
            _provider = provider;
            _logger = logger;

            var assemblyPath = FileExtensions.GetFullPath(option.Response.Body.Props.GetString(ASSEMBLY_NAME_PROPS));
            _plugin = ResolvePlugin(assemblyPath, option.Response.Body.Props.GetString(ASSEMBLY_CLASS_PROPS));
        }

        public override async Task Validate(HttpContext context, MockOption option)
        {
            await base.Validate(context, option);
            await _plugin?.Validate(context, option);
        }

        protected override async Task<string> GetBody()
        {
            return ReplaceWithRegex(await _plugin?.Execute());
        }

        /// <summary>
        /// Resolve plugin method where assembly resolves againts UNC path and returned as an IMockPlugin instance
        /// </summary>
        /// <param name="pluginPath">Full UNC path to the plugin assembly</param>
        /// <returns></returns>
        private IMockPlugin ResolvePlugin(string pluginPath, string @class = null)
        {
            try
            {
                AssemblyLoadContext cntx = new("assembly_context");
                Assembly pluginAssembly = cntx.LoadFromAssemblyPath(pluginPath);

                if (pluginAssembly == null || !pluginAssembly.GetTypes().Any(x => x.GetInterfaces().Contains(typeof(IMockPlugin))))
                {
                    throw new FileNotFoundException($"File '{pluginPath}' not found");
                }

                var cls = !string.IsNullOrWhiteSpace(@class)
                    ? pluginAssembly.GetTypes().FirstOrDefault(x => x.Name.Equals(@class, StringComparison.InvariantCultureIgnoreCase))
                    : pluginAssembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IMockPlugin)));

                return (IMockPlugin)ActivatorUtilities.CreateInstance(_provider, cls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
