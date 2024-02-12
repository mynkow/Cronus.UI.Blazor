namespace Elders.Cronus.Dashboard.Models
{
    public class ProjectionStatus
    {
        public const string Live = "live";
        public const string NotPresent = "not_present";
        public const string Timedout = "timedout";
        public const string Canceled = "canceled";
        public const string Paused = "paused";
        public const string Fixing = "fixing";
        public const string New = "new";

        // OLD
        [Obsolete("Use Fixing instead")]
        public const string Building = "building";

        [Obsolete("Use Fixing instead")]
        public const string Rebuilding = "rebuilding";

        [Obsolete("Use New instead.")]
        public const string Replaying = "replaying";
    }
}
