namespace Elders.Cronus.Dashboard.Models
{
    public class ProjectionVersion
    {
        public string Hash { get; set; }

        public int Revision { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return $"{Status}-{Hash}-{Revision}";
        }
    }
}
