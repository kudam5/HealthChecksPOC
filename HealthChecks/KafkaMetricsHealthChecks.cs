using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HealthMonitorPOC.Web.HealthChecks
{
    public class kafkaMetricsHealthChecks : IHealthCheck
    {
        private readonly IOptions<Settings.KafkaSettings> _kafkaSettings;
        private readonly IOptions<Settings.MetricSettings> _metrics;
        private readonly IAPIService _apiService;

        public kafkaMetricsHealthChecks(IOptions<KafkaSettings> kafkaSettings, IOptions<MetricSettings> metrics, IAPIService apiService)
        {
            _kafkaSettings = kafkaSettings;
            _metrics = metrics;
            _apiService = apiService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return HealthCheckResult.Unhealthy(description: "Cancellation token requested");

            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", _kafkaSettings.Value.MetricsAuthorization);

            try
            {
                var data = _kafkaSettings.Value.MetricsPayload;

                var payload = JsonSerializer.Serialize(data);
                var result = await _apiService.PostAsync(new Uri(_kafkaSettings.Value.MetricsEndpoint), authenticationHeaderValue, payload);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var datum = JsonSerializer.Deserialize<KakfaResult>(response);
                    if (datum?.data is not null)
                    {
                        var value = datum.data.Sum(x => x.value);

                        if (value <= _metrics.Value.KafkaMessageCountMetric)
                        {
                            return HealthCheckResult.Degraded(description: $"Count is too low: {value}");
                        }
                        else
                        {
                            return HealthCheckResult.Healthy(description: $"Count is appears normal: {value}");
                        }
                    }
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
