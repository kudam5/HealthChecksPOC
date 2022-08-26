using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.Helpers;
using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace HealthMonitorPOC.Web.HealthChecks
{
    public class APIHealthCheck : IHealthCheck
    {
        private readonly IOptions<Settings.WeatherForecastSettings> _settings;
        private readonly IOptions<Settings.MetricSettings> _metrics;
        private readonly IAPIService _apiService;

        public APIHealthCheck(IOptions<Settings.WeatherForecastSettings> settings, IOptions<MetricSettings> metrics, IAPIService apiService)
        {
            _settings = settings;
            _metrics = metrics;
            _apiService = apiService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)            
                return HealthCheckResult.Unhealthy(description: "Cancellation token requested");            

            var endpoint = HealthCheckHelper.SetupEndpoint(_settings.Value.WeatherForecastAPIEndpoint);        

            try
            {
                Stopwatch sw = new Stopwatch();                
                sw.Start();
              
                var result = await _apiService.GetAsync(endpoint).ConfigureAwait(false);

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
