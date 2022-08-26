namespace HealthMonitorPOC.Web.Models
{
    public class ElasticLogger
    {
        public string? Request { get; set; }
        public string? Action { get; set; }
        public string? ServerName { get; set; }
        public long? Duration { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? StatusCode { get; set; }
    }
}
