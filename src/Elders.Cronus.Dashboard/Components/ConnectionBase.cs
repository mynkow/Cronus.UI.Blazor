using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
//using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Dashboard.Components
{
    public class ConnectionBase : ComponentBase
    {
        [Inject]
        protected ILogger<ConnectionBase> Log { get; set; }

        [Inject]
        protected ILocalStorageService LocalStorage { get; set; }

        [Inject]
        protected TokenClient Token { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string CronusEndpoint { get; set; }

        [Parameter]
        public string oAuthEndpoint { get; set; }

        [Parameter]
        public string oAuthClient { get; set; }

        [Parameter]
        public string oAuthSecret { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        protected List<Connection> connections;
        protected Connection connection;

        protected async Task<bool> LoadDataAsync()
        {
            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);

            if (string.IsNullOrEmpty(Name) == false)
            {
                connection = connections.Where(conn => conn.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                Name = connection.Name;
                CronusEndpoint = connection.CronusEndpoint;
                oAuthEndpoint = connection.oAuth.ServerEndpoint;
                oAuthClient = connection.oAuth.Client;
                oAuthSecret = connection.oAuth.Secret;
            }

            return true;
        }

        protected async Task EditConnection()
        {
            if (connections.Remove(connection))
            {
                var newConnection = GetConnection();
                connections.Add(newConnection);
                await LocalStorage.SetItemAsync(LSKey.Connections, connections);
                connection = newConnection;
            }

            StateHasChanged();
        }

        protected async Task AddConnection()
        {
             var connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections) ?? new List<Connection>();

            var newConnection = GetConnection();
            connections.Add(newConnection);
            await LocalStorage.SetItemAsync(LSKey.Connections, connections);

            StateHasChanged();
        }

        Connection GetConnection()
        {
            return new Connection()
            {
                Name = Name,
                CronusEndpoint = CronusEndpoint,
                oAuth = new oAuth()
                {
                    ServerEndpoint = oAuthEndpoint,
                    Client = oAuthClient,
                    Secret = oAuthSecret
                }
            };
        }

        protected async Task GetToken()
        {
            Log.LogDebug("GetToken()");

            var result = await Token.GetAccessTokenAsync(connection);

            Log.LogDebug(result);
        }

        protected async Task OnDelete(Connection model)
        {
            Log.LogDebug("OnDelete");

            connections.Remove(model);
            await LocalStorage.SetItemAsync(LSKey.Connections, connections);

            StateHasChanged();
        }
    }
}
