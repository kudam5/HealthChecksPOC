namespace HealthMonitorPOC.Web.Settings
{
    public class KafkaSettings
    {
        public string Endpoint { get; set; }
        public string MetricsEndpoint { get; set; }
        public string PublishEndpoint { get; set; }
        public MetricsPayload MetricsPayload { get; set; }
        public string MetricsAuthorization { get; set; }
        public string ClusterAuthorization { get; set; }
        public string TopicName { get; set; }
    }

    public class Aggregation
    {
        public string metric { get; set; }
    }

    public class Filter
    {
        public string op { get; set; }
        public List<Filter2> filters { get; set; }
    }

    public class Filter2
    {
        public string field { get; set; }
        public string op { get; set; }
        public string value { get; set; }
    }

    public class MetricsPayload
    {
        public List<Aggregation> aggregations { get; set; }
        public Filter filter { get; set; }
        public string granularity { get; set; }
        public List<string> intervals { get; set; }
        public int limit { get; set; }
    }

    public class Datum
    {
        public DateTime timestamp { get; set; }
        public double value { get; set; }
    }

    public class KakfaResult
    {
        public List<Datum> data { get; set; }
    }
    public class PublishRequest
    {
        public Value value { get; set; }
    }

    public class Value
    {
        public string type { get; set; }
        public string data { get; set; }
    }




}
