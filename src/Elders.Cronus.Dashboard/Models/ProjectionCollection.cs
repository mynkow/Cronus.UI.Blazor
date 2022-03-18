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
}
