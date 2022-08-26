using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.Helpers;
using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace HealthMonitorPOC.Web.HealthChecks
{
    public class KafkaConnectionHealthCheck : IHealthCheck
    {
        private readonly IOptions<Settings.KafkaSettings> _kafkaSettings;
        private readonly IOptions<Settings.MetricSettings> _metrics;
        private readonly IAPIService _apiService;

        public KafkaConnectionHealthCheck(IOptions<Settings.KafkaSettings> kafkaSettings, IOptions<MetricSettings> metrics, IAPIService apiService)
        {
            _kafkaSettings = kafkaSettings;
            _metrics = metrics;
            _apiService = apiService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            if (cancellationToken.IsCancellationRequested)
                return HealthCheckResult.Unhealthy(description: "Cancellation token requested");

            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", _kafkaSettings.Value.ClusterAuthorization);         
       
            try
            {
                Stopwatch sw = new Stopwatch();              
                sw.Start();

                var result = await _apiService.GetAsync(new Uri(_kafkaSettings.Value.Endpoint), authenticationHeaderValue);

                sw.Stop();

                return HealthCheckHelper.EvaluateHealthCheck(result, sw, _metrics.Value.MaxAverageResponseTime);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(description: ex.Message);
            }
        }
    }
}
