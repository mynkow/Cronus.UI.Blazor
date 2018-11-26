namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
    }

    public class Connection
    {
        public string Name { get; set; }

        public string CronusEndpiont { get; set; }

        public oAuth oAuth { get; set; }
    }

    public class oAuth
    {
        public string ServerEndpoint { get; set; }

        public string Client { get; set; }

        public string Secret { get; set; }
    }
}
