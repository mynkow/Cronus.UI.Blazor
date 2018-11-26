using Blazor.Extensions.Logging;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Dashboard
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(builder => builder
                .AddBrowserConsole() // Add Blazor.Extensions.Logging.BrowserConsoleLogger
                .SetMinimumLevel(LogLevel.Debug)
            );
            services.AddStorage();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
