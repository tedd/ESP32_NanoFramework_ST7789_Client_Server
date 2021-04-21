using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tedd.House1.DeviceServer.Services;

namespace Tedd.House1.DeviceServer
{
class Program
{

    static async Task<int> Main(string[] args)
    {
        var startup = new Startup();
        Host
            // * set the ContentRootPath to the result of GetCurrentDirectory()
            // * load host IConfiguration from "DOTNET_" prefixed environment variables
            // * load app IConfiguration from 'appsettings.json' and 'appsettings.[EnvironmentName].json'
            // * load app IConfiguration from User Secrets when EnvironmentName is 'Development' using the entry assembly
            // * load app IConfiguration from environment variables
            // * configure the ILoggerFactory to log to the console, debug, and event source output
            // * enables scope validation on the dependency injection container when EnvironmentName is 'Development'
            .CreateDefaultBuilder(args)
            .ConfigureServices(startup.ConfigureServices)
            .UseConsoleLifetime()
            .Build()
            .Run();

        return 0;
    }

}

    internal class Startup
    {
        public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<NetworkDeviceServerService>();
        }

    }
}
