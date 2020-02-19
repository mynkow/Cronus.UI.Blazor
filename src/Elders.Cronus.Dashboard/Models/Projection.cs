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

        public List<ProjectionVersion> Versions { get; set; }

        public ProjectionVersion LiveVersion => Versions.Where(x => x.Status.Equals(ProjectionStatus.Live)).SingleOrDefault();

        public ProjectionVersion LatestVersion => Versions.OrderByDescending(x => x.Revision).First();
    }

    public class EventStoreIndex
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }
    }
}
