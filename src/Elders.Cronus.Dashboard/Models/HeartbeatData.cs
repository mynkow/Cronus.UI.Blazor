namespace Elders.Cronus.Hosting.Heartbeat
{
    public class HeartbeatData
    {
        HeartbeatData() { }

        public string Tenant { get; set; }

        public string BoundedContext { get; set; }

        public List<string> Tenants { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string MachineName { get; set; }

        public string EnvironmentConfig { get; set; }
    }
}

