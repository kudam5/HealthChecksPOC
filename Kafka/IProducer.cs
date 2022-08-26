using HealthMonitorPOC.Web.Settings;

namespace HealthMonitorPOC.Web.Kafka
{
    public interface IKafkaProducer
    {
        public Task<string> GetEvents();
        public Task<string> PublishEvent(PublishRequest model);
    }
}
