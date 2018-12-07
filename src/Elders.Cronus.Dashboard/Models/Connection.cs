using System;
using System.Text;

namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
    }

    public class Connection
    {
        public string Name { get; set; }

        public string CronusEndpoint { get; set; }

        public oAuth oAuth { get; set; }
    }

    public class oAuth
    {
        public string ServerEndpoint { get; set; }

        public string Client { get; set; }

        public string Secret { get; set; }

        public string BasicAuthorization => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Client}:{Secret}"));
    }
}
