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

        public bool IsReplayable { get; set; }

        public List<ProjectionVersion> Versions { get; set; }

        public ProjectionVersion LiveVersion => Versions.Where(x => x.Status.Equals(ProjectionStatus.Live)).LastOrDefault();

        public ProjectionVersion LatestVersion => Versions.OrderByDescending(x => x.Revision).First();
    }
}
