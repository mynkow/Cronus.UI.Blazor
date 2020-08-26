using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Elders.Cronus.Dashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddLoadingBar();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }.EnableIntercept(sp));

            builder.Services.AddSingleton<AppState>();

            builder.Services.AddHttpClient<CronusClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                // If you want to use Typed Client, please invoke "EnableIntercept()" here.
                client.EnableIntercept(sp);
            });


           // builder.Services.AddScoped<CronusClient>();
            builder.Services.AddTransient<TokenClient>();

            builder.Services.AddBlazoredLocalStorage();
            

            await builder
                .Build()
                .UseLoadingBar()
                .RunAsync();
        }
    }
}
