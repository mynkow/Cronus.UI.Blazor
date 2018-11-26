using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Blazor.Components;

namespace Elders.Cronus.Dashboard.Pages
{
    public class ConnectionsBase : BlazorComponent
    {
        protected List<Connection> connections;

        [Inject]
        public LocalStorage LocalStorage { get; set; }

        public ConnectionsBase()
        {
            this.connections = new List<Connection>();
        }

        protected override async Task OnInitAsync()
        {
            connections = await LocalStorage.GetItem<List<Connection>>(LSKey.Connections);
        }

        protected async Task OnDelete(Connection model)
        {
            connections.Remove(model);
            await LocalStorage.SetItem(LSKey.Connections, connections);

            StateHasChanged();
        }
    }
}
