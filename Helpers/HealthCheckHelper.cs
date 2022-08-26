using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HealthMonitorPOC.Web.Helpers
{
    public static class HealthCheckHelper
    {
        public static Uri SetupEndpoint(string endpoint)
        {
            Uri uri = new Uri(endpoint);
            return uri;
        }

        public static HealthCheckResult EvaluateHealthCheck(HttpResponseMessage httpResponseMessage, Stopwatch stopwatch, int maximumAverageResponse)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (stopwatch.ElapsedMilliseconds > maximumAverageResponse)
                    return HealthCheckResult.Degraded(description: $"Duration: {stopwatch.ElapsedMilliseconds}");

                return HealthCheckResult.Healthy(description: $"Duration: {stopwatch.ElapsedMilliseconds}");
            }

            return HealthCheckResult.Unhealthy(description: httpResponseMessage.StatusCode.ToString());
        }

        public static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description)))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
