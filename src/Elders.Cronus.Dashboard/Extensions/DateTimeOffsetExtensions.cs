namespace Elders.Cronus.Dashboard.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        private const string Iso8601UtcDateTimeFormat = "yyyy-MM-ddTHH:mm:sssZ";

        public static string ToIso8601(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToUniversalTime().ToString(Iso8601UtcDateTimeFormat);
        }
    }
}
