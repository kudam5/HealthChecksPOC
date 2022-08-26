namespace HealthMonitorPOC.Web.ElasticSearch
{
    public interface IElasticEvents<T>
    {
        public Task<bool> Publish(T payload);
    }
}
