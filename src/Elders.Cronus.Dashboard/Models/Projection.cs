namespace Elders.Cronus.Dashboard.Models
{
    public class Projection
    {
        public Projection()
        {
            Versions = new List<ProjectionVersion>();
        }

        public string ProjectionContractId { get; set; }

        public string ProjectionName { get; set; }

        public bool IsReplayable { get; set; } = true;

        public List<ProjectionVersion> Versions { get; set; }

        public ProjectionVersion LiveVersion => Versions.Where(x => x.Status.Equals(ProjectionStatus.Live)).LastOrDefault();

        public ProjectionVersion LatestVersion => Versions.OrderByDescending(x => x.Revision).First();

        public ProjectionVersion RebuildingVersion => Versions.Where(x => x.Status.Equals(ProjectionStatus.Rebuilding) || x.Status.Equals(ProjectionStatus.Building))
            .OrderBy(x => x.Revision).LastOrDefault() ?? LiveVersion;

        public List<ProjectionVersion> RebuildingVersions => Versions.Where(x => x.Status.Equals(ProjectionStatus.Rebuilding) || x.Status.Equals(ProjectionStatus.Building)).ToList();

        public List<ProjectionVersion> ReplayingVersions => Versions.Where(x => x.Status.Equals(ProjectionStatus.Replaying)).ToList();
    }
}
