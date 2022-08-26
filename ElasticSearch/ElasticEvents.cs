using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Options;

namespace HealthMonitorPOC.Web.ElasticSearch
{
    public class ElasticEvents<T> : IElasticEvents<T> where T : class
    {
        private readonly IOptions<ElasticSearchSettings> _elasticSearchSettings;

        public ElasticEvents(IOptions<ElasticSearchSettings> elasticSearchSettings)
        {
            _elasticSearchSettings = elasticSearchSettings;
        }

        public async Task<bool> Publish(T payload)
        {
            var node = new Uri(_elasticSearchSettings.Value.Endpoint);
            var settings = new Nest.ConnectionSettings(node);
            var client = new Nest.ElasticClient(settings);

            client.Indices.Create(_elasticSearchSettings.Value.IndexName,
                index => index.Map<T>(
                    x => x.AutoMap()));

            await client.IndexAsync(payload, idx => idx.Index(_elasticSearchSettings.Value.IndexName));

            return true;
        }
    }
}
