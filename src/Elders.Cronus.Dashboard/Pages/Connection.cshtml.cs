using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Blazor.Components;

namespace Elders.Cronus.Dashboard.Pages
{
    public class ConnectionBase : BlazorComponent
    {
        protected string name;
        protected string cronusEndpoint;

        [Inject]
        protected LocalStorage LocalStorage { get; set; }

        public async Task AddConnection()
        {
            ConnectionModel connection = new ConnectionModel();
            connection.Name = name;
            connection.CronusEndpiont = cronusEndpoint;

            var connections = await LocalStorage.GetItem<List<ConnectionModel>>(LSKey.Connections) ?? new List<ConnectionModel>();
            connections.Add(connection);
            await LocalStorage.SetItem<List<ConnectionModel>>(LSKey.Connections, connections);
        }
    }

}
