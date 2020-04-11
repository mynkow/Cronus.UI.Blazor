//using Blazor.Extensions.Logging;
using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Elders.Cronus.Dashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging(builder => builder
            //    .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
            //    .SetMinimumLevel(LogLevel.Debug)
            //);
            services.AddLoadingBar();
            services.AddStorage();
            services.AddTransient<CronusClient>();
            services.AddTransient<TokenClient>();
            services.AddSingleton<AppState>();
        }
    }
}
