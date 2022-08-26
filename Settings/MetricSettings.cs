namespace HealthMonitorPOC.Web.Settings
{
    public class MetricSettings
    {
        public int MaxAverageResponseTime { get; set; }
        public int ElasticDurationMetric { get; set; }
        public int KafkaMessageCountMetric { get; set; }
    }
}
