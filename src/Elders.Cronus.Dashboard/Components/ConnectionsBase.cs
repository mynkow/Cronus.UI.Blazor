using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

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

        [Inject]
        public IJSRuntime JSRuntime { get; set; }


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

        protected async Task ExportAsync()
        {
            var connections = await LocalStorage.GetItemAsync<List<Connection>>(LSKey.Connections);
            var json = JsonSerializer.Serialize(connections, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            byte[] file = Encoding.UTF8.GetBytes(json);
            string fileName = "cronus dashboard connections.json";
            string contentType = "application/json";

            // Check if the IJSRuntime is the WebAssembly implementation of the JSRuntime
            if (JSRuntime is IJSUnmarshalledRuntime webAssemblyJSRuntime)
            {
                webAssemblyJSRuntime.InvokeUnmarshalled<string, string, byte[], bool>("BlazorDownloadFileFast", fileName, contentType, file);
            }
            else
            {
                // Fall back to the slow method if not in WebAssembly
                await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            }
        }

        protected async Task ImportAsync(InputFileChangeEventArgs e)
        {
            using var stream = e.File.OpenReadStream();
            using var streamReaded = new StreamReader(stream);

            var json = await streamReaded.ReadToEndAsync();
            connections = JsonSerializer.Deserialize<List<Connection>>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            await LocalStorage.SetItemAsync(LSKey.Connections, connections);
            App.UpdateConnections(connections);
            StateHasChanged();
        }
    }
}
