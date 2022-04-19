using System.Text;
using System.Text.Json.Serialization;

namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
        public const string SelectedConnection = "selected-connection";
        public const string HubConnection = "hub-connection";
    }

    public class Connection
    {
        public Connection(string name, string cronusEndpoint, oAuth oAuth = null, bool isAutoConnected = false)
        {
            Name = name;
            CronusEndpoint = cronusEndpoint;
            IsAutoConnected = isAutoConnected;
            oAuths = new List<oAuth>();
            this.oAuth = oAuth ?? new oAuth();
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cronusEndpoint")]
        public string CronusEndpoint { get; set; }

        [JsonPropertyName("oAuth")]
        public oAuth oAuth { get; set; }

        [JsonPropertyName("isAutoConnected")]
        public bool IsAutoConnected { get; set; }

        [JsonPropertyName("oAuths")]
        public List<oAuth> oAuths { get; set; }
    }

    public class oAuth
    {
        public oAuth() { }

        public oAuth(string serverEndpoint, string client, string secret, string scope, string tenant)
        {
            ServerEndpoint = serverEndpoint;
            Client = client;
            Secret = secret;
            Scope = scope;
            Tenant = tenant;
        }

        [JsonPropertyName("serverEndpoint")]
        public string ServerEndpoint { get; set; }

        [JsonPropertyName("client")]
        public string Client { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        public string BasicAuthorization => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Client}:{Secret}"));

        [JsonPropertyName("tenant")]
        public string Tenant { get; set; }
    }
}
