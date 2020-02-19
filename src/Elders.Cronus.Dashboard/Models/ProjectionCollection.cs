using System.Collections.Generic;

namespace Elders.Cronus.Dashboard.Models
{
    public class ProjectionCollection
    {
        public ProjectionCollection()
        {
            Projections = new List<Projection>();
        }

        public List<Projection> Projections { get; set; }
    }

    public class EventStoreIndexCollection
    {
        public EventStoreIndexCollection()
        {
            Indices = new List<EventStoreIndex>();
        }

        public List<EventStoreIndex> Indices { get; set; }
    }
}
