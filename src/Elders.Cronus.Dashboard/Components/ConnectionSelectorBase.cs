using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Elders.Cronus.Dashboard.Pages;
using Microsoft.AspNetCore.Components;

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
            App.OnAutoConnect += OnConnectionSelected;
            App.OnTenantChanged += OnAutoTenantChanged;
            App.OnDisconnect += Disconenct;
            App.OnChange += () => StateHasChanged();

            TenantName = App.oAuth?.Tenant ?? "Select Tenant...";
            connections = new List<Connection>();
            oAuths = new List<oAuth>();

            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);
            App.LoadConnections(connections);
        }

        protected async Task OnConnectionSelected(IEnumerable<Connection> connections)
        {
            var connection = connections.FirstOrDefault();
            Connection = connection;
            Task connectTask = App.ConnectAsync(connection); // keep not awaited
            TenantName = App.oAuth?.Tenant ?? "Select Tenant...";
            List<string> configuredTenantsInTheService = await Cronus.GetTenantsAsync(connection);
            List<oAuth> intersection = connection.oAuths.Where(x => configuredTenantsInTheService.Contains(x.Tenant)).ToList();
            oAuths = intersection;
            OAuth = null;
            NavManager.NavigateTo("/");

            StateHasChanged();
        }

        protected async Task OnTenantChanged(IEnumerable<oAuth> oAuths)
        {
            var oAuth = oAuths.FirstOrDefault();
            await App.SelectTenantAsync(oAuth);
            OAuth = oAuth;
            TenantName = oAuth.Tenant;
            NavManager.NavigateTo("/");
            NavManager.NavigateTo("/projections");

            StateHasChanged();
        }

        protected Task OnAutoTenantChanged(oAuth oAuth)
        {
            OAuth = oAuth;
            TenantName = oAuth.Tenant;
            NavManager.NavigateTo("/projections");
            StateHasChanged();

            return Task.CompletedTask;
        }

        protected Task UpdateConnections(List<Connection> updatedConnections)
        {
            connections = updatedConnections;
            StateHasChanged();

            return Task.CompletedTask;
        }

        protected Task Disconenct()
        {
            Connection = null;
            OAuth = null;
            oAuths = null;
            StateHasChanged();

            return Task.CompletedTask;
        }
    }
}
