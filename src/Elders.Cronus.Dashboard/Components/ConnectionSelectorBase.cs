using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
//using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Elders.Cronus.Dashboard.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Dashboard.Components
{
    public class ConnectionSelectorBase : ComponentBase
    {
        [Inject]
        public AppState App { get; set; }

        [Inject]
        public ILogger<ConnectionSelector> Log { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public CronusClient Cronus { get; set; }

        [Parameter]
        public string ConnectionName { get; set; }

        [Parameter]
        public string TenantName { get; set; }

        protected List<Connection> connections { get; set; }
        protected List<oAuth> oAuths { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ConnectionName = App.Connection?.Name ?? "Select Connection...";
            TenantName = App.oAuth?.Tenant ?? "Select Tenant...";
            connections = new List<Connection>();
            oAuths = new List<oAuth>();

            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);
        }

        protected async Task OnConnectionClick(Connection connection)
        {
            App.Connect(connection);
            ConnectionName = connection.Name;

            List<string> configuredTenantsInTheService = await Cronus.GetTenantsAsync(connection);
            List<oAuth> intersection = connection.oAuths.Where(x => configuredTenantsInTheService.Contains(x.Tenant)).ToList();

            oAuths = intersection;
            StateHasChanged();
        }

        protected async Task OnAuthSelected(oAuth oAuth)
        {
            App.SelectTenant(oAuth);
            TenantName = oAuth.Tenant;

            StateHasChanged();
        }
    }
}
