using HealthMonitorPOC.Web.ElasticSearch;
using HealthMonitorPOC.Web.Models;
using System.Diagnostics;

namespace HealthMonitorPOC.Web.Logging
{
    public class ResponseLogMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IElasticEvents<ElasticLogger> _elasticEvents;
        public ResponseLogMiddleware(RequestDelegate requestDelegate, IElasticEvents<ElasticLogger> elasticEvents)
        {
            _elasticEvents = elasticEvents;
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            long duration = 0;

            var sw = Stopwatch.StartNew();

            await _requestDelegate(httpContext);

            duration = sw.ElapsedMilliseconds;

            var logger = new ElasticLogger
            {
                Request = $"{httpContext.Request.Host} {httpContext.Request.Path}",
                StatusCode = httpContext.Response.StatusCode,
                ServerName = Environment.MachineName,
                Action = httpContext.Request.Method,
                TimeStamp = DateTime.UtcNow,
                Duration = duration                
            };

            await _elasticEvents.Publish(logger);
        }
    } 
}
