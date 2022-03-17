using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Elders.Cronus.Dashboard.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Windows.Input;

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

        [Inject]
        public NavigationManager NavManager { get; set; }

        //[Parameter]
        //public string ConnectionName { get; set; }

        [Parameter]
        public Connection Connection { get; set; }

        [Parameter]
        public oAuth OAuth { get; set; }

        [Parameter]
        public string TenantName { get; set; }

        protected List<Connection> connections { get; set; }

        protected List<oAuth> oAuths { get; set; }

        protected override async Task OnInitializedAsync()
        {
            App.OnConnectionsUpdated += UpdateConnections;

            //ConnectionName = App.Connection?.Name ?? "Select Connection...";
            TenantName = App.oAuth?.Tenant ?? "Select Tenant...";
            connections = new List<Connection>();
            oAuths = new List<oAuth>();

            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);
            App.LoadConnections(connections);
        }

        protected async Task OnConnectionSelected(IEnumerable<Connection> connections)
        {
            var connection = connections.FirstOrDefault();
            App.Connect(connection);
            //ConnectionName = connection.Name;
            NavManager.NavigateTo("/");
            TenantName = App.oAuth?.Tenant ?? "Select Tenant...";
            List<string> configuredTenantsInTheService = await Cronus.GetTenantsAsync(connection);
            List<oAuth> intersection = connection.oAuths.Where(x => configuredTenantsInTheService.Contains(x.Tenant)).ToList();

            oAuths = intersection;
            StateHasChanged();
        }

        protected void OnTenantChanged(IEnumerable<oAuth> oAuths)
        {
            var oAuth = oAuths.FirstOrDefault();
            App.SelectTenant(oAuth);
            TenantName = oAuth.Tenant;
            StateHasChanged();
        }

        protected async Task UpdateConnections(List<Connection> updatedConnections)
        {
            connections = updatedConnections;
            StateHasChanged();
        }
    }
}
