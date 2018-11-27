using System.Collections.Generic;

namespace Elders.Cronus.Dashboard.Models
{
    public class ProjectionsResult
    {
        public ProjectionsResult()
        {
            Projections = new List<Projection>();
        }

        public List<Projection> Projections { get; set; }
    }
}
