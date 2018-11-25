using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Blazor.Components;

namespace Elders.Cronus.Dashboard.Pages
{
    public class ConnectionsBase : BlazorComponent
    {
        protected List<ConnectionModel> connections;

        [Inject]
        public LocalStorage LocalStorage { get; set; }

        public ConnectionsBase()
        {
            this.connections = new List<ConnectionModel>();
        }

        protected override async Task OnInitAsync()
        {
            connections = await LocalStorage.GetItem<List<ConnectionModel>>(LSKey.Connections);
        }
    }
}
