using Elders.Cronus.Dashboard;
using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddLoadingBar();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }.EnableIntercept(sp));

builder.Services.AddSingleton<AppState>();

builder.Services.AddHttpClient<CronusClient>((sp, client) =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    // If you want to use Typed Client, please invoke "EnableIntercept()" here.
    client.EnableIntercept(sp);
});

builder.Services.AddTransient<TokenClient>();
builder.Services.AddBlazoredLocalStorage();

await builder
    .UseLoadingBar()
    .Build()
    .RunAsync();

