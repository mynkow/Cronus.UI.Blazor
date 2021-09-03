using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Dashboard.Components
{
    public class ConnectionsBase : ComponentBase
    {
        protected List<Connection> connections;

        [Inject]
        public AppState App { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public ILogger<ConnectionsBase> Logger { get; set; }


        public ConnectionsBase()
        {
            this.connections = new List<Connection>();
        }

        protected override async Task OnInitializedAsync()
        {
            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);
            if (connections is null)
                connections = new List<Connection>();
        }

        protected async Task OnDeleteAsync(Connection model)
        {
            connections.Remove(model);
            await LocalStorage.SetItemAsync(LSKey.Connections, connections);
            App.UpdateConnections(connections);

            Logger.LogInformation($"{model.Name} has been deleted.");

            StateHasChanged();
        }
    }
}
