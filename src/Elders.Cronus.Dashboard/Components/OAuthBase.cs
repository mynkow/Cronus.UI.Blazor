using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace Elders.Cronus.Dashboard.Components
{
    public class OAuthBase : ComponentBase
    {
        [Inject]
        protected AppState App { get; set; }

        [Inject]
        protected ILogger<ConnectionBase> Log { get; set; }

        [Inject]
        protected ILocalStorageService LocalStorage { get; set; }

        [Inject]
        protected TokenClient Token { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected CronusClient Cronus { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string ConnectionName { get; set; }

        [Parameter]
        public string Tenant { get; set; }

        [Parameter]
        public string Audience { get; set; }

        [Parameter]
        public string ServerEndpoint { get; set; }

        [Parameter]
        public string Client { get; set; }

        [Parameter]
        public string Secret { get; set; }

        [Parameter]
        public string Scope { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        protected List<Connection> connections;
        protected Connection connection;
        protected oAuth oAuth;

        protected async Task<bool> LoadDataAsync()
        {
            connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);

            if (string.IsNullOrEmpty(ConnectionName) == false)
            {
                connection = connections.Where(conn => conn.Name.Equals(ConnectionName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                oAuth = connection.oAuths.Where(oAuth => oAuth.Tenant.Equals(Tenant, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                Log.LogInformation(oAuth.Client);
                Log.LogInformation(oAuth.Secret);
                Log.LogInformation(oAuth.Scope);
                Log.LogInformation(oAuth.ServerEndpoint);
                Log.LogInformation(oAuth.Audience);
                Log.LogInformation(oAuth.Tenant);

                ServerEndpoint = oAuth.ServerEndpoint;
                Client = oAuth.Client;
                Secret = oAuth.Secret;
                Scope = oAuth.Scope;
                Audience = oAuth.Audience;
            }

            return true;
        }

        protected async Task EditTenant()
        {
            if (connections.Remove(connection))
            {
                oAuth changedAuthentication = GetoAuth();
                connection.oAuths.Remove(oAuth);
                connection.oAuths.Add(changedAuthentication);
                connection.oAuth = changedAuthentication;

                connections.Add(connection);
                await LocalStorage.SetItemAsync(LSKey.Connections, connections);
            }

            App.UpdateConnections(connections);
            await LoadDataAsync();

            StateHasChanged();
            NavigationManager.NavigateTo($"/connection/{ConnectionName}");
        }

        //protected async Task OnDeleteAsync(Connection model)
        //{
        //    connections.Remove(model);
        //    await LocalStorage.SetItemAsync(LSKey.Connections, connections);

        //    Logger.LogInformation($"{model.Name} has been deleted.");

        //    StateHasChanged();
        //}

        private oAuth GetoAuth()
        {
            return new oAuth(ServerEndpoint, Client, Secret, Scope, Audience, Tenant);
        }
    }
}
