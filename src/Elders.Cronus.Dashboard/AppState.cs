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

        public Connection Connection { get; private set; }

        public void Connect(Connection connection)
        {
            Connection = connection;
            NotifyStateChanged();
            NotifyConnectionChanged(Connection);
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        private Task NotifyConnectionChanged(Connection connection) => OnConnectionChanged?.Invoke(connection);
    }
}
