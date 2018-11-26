using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.Extensions.Logging;

namespace Elders.Cronus.Dashboard.Components
{
    public class ConnectionBase : BlazorComponent
    {
        [Inject]
        protected ILogger<ConnectionBase> Log { get; set; }

        [Inject]
        protected LocalStorage LocalStorage { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Parameter]
        protected string Name { get; set; }

        [Parameter]
        protected string CronusEndpoint { get; set; }

        [Parameter]
        protected string oAuthEndpoint { get; set; }

        [Parameter]
        protected string oAuthClient { get; set; }

        [Parameter]
        protected string oAuthSecret { get; set; }

        protected override async Task OnInitAsync()
        {
            await LoadDataAsync();
        }

        protected List<Connection> connections;
        protected Connection connection;

        protected async Task<bool> LoadDataAsync()
        {
            connections = await LocalStorage.GetItem<List<Connection>>(LSKey.Connections);

            if (string.IsNullOrEmpty(Name) == false)
            {
                connection = connections.Where(conn => conn.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                Name = connection.Name;
                CronusEndpoint = connection.CronusEndpiont;
                oAuthEndpoint = connection.oAuth.ServerEndpoint;
                oAuthClient = connection.oAuth.Client;
                oAuthSecret = connection.oAuth.Secret;
            }

            return true;
        }

        protected async Task EditConnection()
        {
            connections.Remove(connection);
            var newConnection = GetConnection();
            connections.Add(newConnection);
            await LocalStorage.SetItem<List<Connection>>(LSKey.Connections, connections);
        }

        protected async Task AddConnection()
        {
            var connections = await LocalStorage.GetItem<List<Connection>>(LSKey.Connections) ?? new List<Connection>();

            var newConnection = GetConnection();
            connections.Add(newConnection);
            await LocalStorage.SetItem<List<Connection>>(LSKey.Connections, connections);
        }

        Connection GetConnection()
        {
            return new Connection()
            {
                Name = Name,
                CronusEndpiont = CronusEndpoint,
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

            HttpRequestMessage getTokenRequest = new HttpRequestMessage(HttpMethod.Post, connection.oAuth.ServerEndpoint);
            getTokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connection.oAuth.Client}:{connection.oAuth.Secret}")));
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("scope", "read");
            getTokenRequest.Content = new FormUrlEncodedContent(parameters);
            var disco = await HttpClient.SendAsync(getTokenRequest);
            var result = await disco.Content.ReadAsStringAsync();
            Log.LogDebug(result);
        }

        protected async Task OnDelete(Connection model)
        {
            Log.LogDebug("OnDelete");

            connections.Remove(model);
            await LocalStorage.SetItem(LSKey.Connections, connections);

            StateHasChanged();
        }
    }
}
