using System.Collections.Generic;
using System.Linq;

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

        public List<ProjectionVersion> RebuildingVersions => Versions.Where(x => x.Status.Equals(ProjectionStatus.Rebuilding)).ToList();

        public List<ProjectionVersion> ReplayingVersions => Versions.Where(x => x.Status.Equals(ProjectionStatus.Replaying)).ToList();

        /// <summary>
        /// For backwards compatibility (Replaying used to be building)
        /// </summary>
        public List<ProjectionVersion> BuildingVersions => Versions.Where(x => x.Status.Equals(ProjectionStatus.Building)).ToList();
    }
}
