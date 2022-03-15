using System.Text;

namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
        public const string SelectedConnection = "selected-connection";
    }

    public class Connection
    {
        public Connection(string name, string cronusEndpoint, oAuth oAuth = null)
        {
            Name = name;
            CronusEndpoint = cronusEndpoint;
            oAuths = new List<oAuth>();
            this.oAuth = oAuth ?? new oAuth();
        }

        public string Name { get; set; }

        public string CronusEndpoint { get; set; }

        public oAuth oAuth { get; set; }

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

        public string ServerEndpoint { get; set; }

        public string Client { get; set; }

        public string Secret { get; set; }

        public string Scope { get; set; }

        public string BasicAuthorization => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Client}:{Secret}"));

        public string Tenant { get; set; }
    }
}
