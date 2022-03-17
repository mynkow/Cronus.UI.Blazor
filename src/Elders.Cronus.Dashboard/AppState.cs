using Elders.Cronus.Dashboard.Components;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Elders.Cronus.Dashboard
{
    public class AppState : IDisposable
    {
        private readonly ILogger<AppState> _logger;
        public AppState(ILogger<AppState> logger)
        {
            _logger = logger;
        }

        // Lets components receive change notifications. Could have whatever granularity you want (more events, hierarchy...)
        public event Action OnChange;
        public event Func<Connection, Task> OnConnectionChanged;
        public event Func<oAuth, Task> OnTenantChanged;
        public event Func<List<Connection>, Task> OnConnectionsUpdated;
        public event Func<ProjectionVersion, Task> OnVersionSelected;
        public event Func<ProgressData, Task> OnProgressChanged;

        public Connection Connection { get; private set; }
        public string Tenant { get; private set; }
        public oAuth oAuth { get; private set; }
        public List<Connection> AvailableConnections { get; private set; }
        public HubConnection HubConnection { get; set; }

        public void Connect(Connection connection)
        {
            Connection = connection;
            oAuth = null;
        }

        public void SelectTenant(oAuth selectedoAuth)
        {
            oAuth = selectedoAuth;
            Connection.oAuth = selectedoAuth;
            NotifyStateChanged();
            NotifyTenantChanged(oAuth);
        }

        public void LoadConnections(List<Connection> connections)
        {
            AvailableConnections = connections;
            NotifyStateChanged();
        }

        public void UpdateConnections(List<Connection> connections)
        {
            AvailableConnections = connections;
            NotifyStateChanged();
            NotifyConnectionsUpdated(connections);
        }

        public void SelectVersion(ProjectionVersion version)
        {
            NotifyVersionSelected(version);
        }

        public void ChangeProgressState(ProgressData progress)
        {
            NotifyProgressChanged(progress);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        private Task NotifyConnectionChanged(Connection connection) => OnConnectionChanged?.Invoke(connection);

        private Task NotifyTenantChanged(oAuth tenant) => OnTenantChanged?.Invoke(tenant);

        private Task NotifyConnectionsUpdated(List<Connection> connections) => OnConnectionsUpdated?.Invoke(connections);

        private Task NotifyVersionSelected(ProjectionVersion version) => OnVersionSelected?.Invoke(version);

        private Task NotifyProgressChanged(ProgressData progress) => OnProgressChanged?.Invoke(progress);


        public async Task ConnectToSignalRAsync()
        {
            _logger.LogInformation("Initializing signalR...");

            HubConnection = new HubConnectionBuilder()
            .WithUrl(Connection.CronusEndpoint + "/hub/projections")
            .WithAutomaticReconnect().Build();

            await HubConnection.StartAsync();
            _logger.LogInformation("SignalR hub started");
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing SignalR hub...");
            HubConnection.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
