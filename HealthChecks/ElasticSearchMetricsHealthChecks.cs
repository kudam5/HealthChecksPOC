using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HealthMonitorPOC.Web.HealthChecks
{
    public class ElasticSearchMetricsHealthChecks : IHealthCheck
    {
        private readonly IOptions<Settings.ElasticSearchSettings> _elasticSearchSettings;
        private readonly IOptions<Settings.MetricSettings> _metrics;
        private readonly IAPIService _apiService;

        public ElasticSearchMetricsHealthChecks(IOptions<Settings.ElasticSearchSettings> elasticSearchSettings, IOptions<Settings.MetricSettings> metrics, IAPIService apiService)
        {
            _elasticSearchSettings = elasticSearchSettings;
            _metrics = metrics;
            _apiService = apiService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return HealthCheckResult.Unhealthy(description: "Cancellation token requested");

            try
            {
                var data = _elasticSearchSettings.Value.ElasticAggregateQuery;

                var payload = JsonSerializer.Serialize(data);
                var result = await _apiService.PostAsync(new Uri($" {_elasticSearchSettings.Value.Endpoint}/{_elasticSearchSettings.Value.ElasticQueryPath}"), null, payload);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();

                    var datum = JsonSerializer.Deserialize<ElasticQueryResult>(response);
                    if (datum?.aggregations is not null)
                    {
                        var value = Math.Round((decimal)(datum?.aggregations?.avg_grade?.value), 2);

                        if (value >= _metrics.Value.KafkaMessageCountMetric)
                        {
                            return HealthCheckResult.Degraded(description: $"Response indicates degraded service. Average response: {value}");
                        }
                        else
                        {
                            return HealthCheckResult.Healthy(description: $"Response indicates degraded service.Average response: { value}");
                        }
                    }
                }
                else
                {
                    return HealthCheckResult.Unhealthy(description: await result.Content.ReadAsStringAsync());
                }

                return HealthCheckResult.Healthy();
               
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description: ex.Message);
            }
        }
    }
}
