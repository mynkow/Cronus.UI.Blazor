using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Storage.Interfaces;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace Elders.Cronus.Dashboard.Pages
{
    public class ConnectionsBase : ComponentBase
    {
        protected List<Connection> connections;

        [Inject]
        public ILocalStorage LocalStorage { get; set; }

        public ConnectionsBase()
        {
            this.connections = new List<Connection>();
        }

        protected override async Task OnInitializedAsync()
        {
            connections = await LocalStorage.GetItem<List<Connection>>(LSKey.Connections);
            if (connections is null)
                connections = new List<Connection>();
        }

        protected async Task OnDelete(Connection model)
        {
            connections.Remove(model);
            await LocalStorage.SetItem(LSKey.Connections, connections);

            StateHasChanged();
        }
    }
}
