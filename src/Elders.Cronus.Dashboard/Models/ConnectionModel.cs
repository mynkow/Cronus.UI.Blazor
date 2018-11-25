namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
    }

    public class ConnectionModel
    {
        public string Name { get; set; }

        public string CronusEndpiont { get; set; }

        public int MyProperty { get; set; }
    }

    public class Authorization
    {
        public string ServerEndpoint { get; set; }

        public string Client { get; set; }

        public string Secret { get; set; }
    }
}
