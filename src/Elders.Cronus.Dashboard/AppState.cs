using System;
using System.Threading.Tasks;
using Elders.Cronus.Dashboard.Models;

namespace Elders.Cronus.Dashboard
{
    public class AppState
    {
        // Lets components receive change notifications. Could have whatever granularity you want (more events, hierarchy...)
        public event Action OnChange;

        public event Func<Connection, Task> OnConnectionChanged;
        public event Func<oAuth, Task> OnTenantChanged;

        public Connection Connection { get; private set; }
        public string Tenant { get; private set; }
        public oAuth oAuth { get; private set; }

        public void Connect(Connection connection)
        {
            Connection = connection;
            oAuth = null;
            NotifyStateChanged();
            NotifyConnectionChanged(Connection);
        }

        public void SelectTenant(oAuth selectedoAuth)
        {
            oAuth = selectedoAuth;
            NotifyStateChanged();
            NotifyTenantChanged(oAuth);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        private Task NotifyConnectionChanged(Connection connection) => OnConnectionChanged?.Invoke(connection);

        private Task NotifyTenantChanged(oAuth tenant) => OnTenantChanged?.Invoke(tenant);
    }
}
