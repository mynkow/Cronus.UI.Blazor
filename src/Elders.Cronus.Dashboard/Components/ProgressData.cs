namespace Elders.Cronus.Dashboard.Components
{
    public class ProgressData
    {
        public ProgressData(string projectionTypeId, string progressString, int processed)
        {
            ProjectionTypeId = projectionTypeId;
            ProgressString = progressString;
            Processed = processed;
        }

        public string ProjectionTypeId { get; set; }
        public string ProgressString { get; set; }
        public int Processed { get; set; }

        public bool IsCompleted => Processed >= 100 ? true : false;
    }
}
