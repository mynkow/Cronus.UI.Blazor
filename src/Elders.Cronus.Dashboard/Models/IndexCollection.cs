namespace Elders.Cronus.Dashboard.Models
{
    public class IndexCollection
    {
        public IndexCollection()
        {
            Indices = new List<Index>();
        }

        public List<Index> Indices { get; set; }
    }

    public class Index
    {
        public string IndexContractId { get; set; }

        public string IndexName { get; set; }

        public string Status { get; set; }

        public int? MaxDegreeOfParallelism { get; set; }
    }
}
