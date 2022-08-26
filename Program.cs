using HealthChecks.Kafka;
using HealthChecks.UI.Client;
using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.ElasticSearch;
using HealthMonitorPOC.Web.HealthChecks;
using HealthMonitorPOC.Web.Helpers;
using HealthMonitorPOC.Web.Kafka;
using HealthMonitorPOC.Web.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(typeof(IAPIService), typeof(APIService));
builder.Services.AddScoped(typeof(IKafkaProducer), typeof(KafkaProducer));
builder.Services.AddSingleton(typeof(IElasticEvents<>), typeof(ElasticEvents<>));

builder.Services.AddHealthChecks()
                .AddElasticsearch(elasticsearchUri: builder.Configuration.GetValue<string>("ElasticSearchSettings:Endpoint"), name: "ElasticSearch", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Services" })
                .AddCheck<APIHealthCheck>(name: "WeatherForecast API", tags: new[] { "API" })
                .AddCheck<KafkaConnectionHealthCheck>(name: "Kafka Connection", tags: new[] { "Services" })
                .AddCheck<kafkaMetricsHealthChecks>(name: "Kafka Metrics", tags: new[] { "Services" })
                .AddCheck<ElasticSearchMetricsHealthChecks>(name: "Elastic Search Metrics", tags: new[] { "Services" })
                .AddSqlServer(connectionString: builder.Configuration.GetValue<string>("ConnectionStrings:LocalDB"), name: "LocalDB", tags: new[] { "Databases"});
                
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.Services.Configure<HealthMonitorPOC.Web.Settings.WeatherForecastSettings>(builder.Configuration.GetSection("Endpoints"));
builder.Services.Configure<HealthMonitorPOC.Web.Settings.MetricSettings>(builder.Configuration.GetSection("Metrics"));
builder.Services.Configure<HealthMonitorPOC.Web.Settings.KafkaSettings>(builder.Configuration.GetSection("KakfaSettings"));
builder.Services.Configure<HealthMonitorPOC.Web.Settings.ElasticSearchSettings>(builder.Configuration.GetSection("ElasticSearchSettings"));

var app = builder.Build();

app.UseMiddleware<ResponseLogMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = HealthCheckHelper.WriteHealthCheckResponse,
        ResultStatusCodes =
        {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status406NotAcceptable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    });

    endpoints.MapHealthChecks("/services", new HealthCheckOptions()
    {
        Predicate = e => e.Tags.Contains("Services"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        ResultStatusCodes =
        {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status406NotAcceptable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    });

    endpoints.MapHealthChecks("/databases", new HealthCheckOptions()
    {
        Predicate = e => e.Tags.Contains("Databases"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        ResultStatusCodes =
            {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status406NotAcceptable,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
    });

    endpoints.MapHealthChecks("/api", new HealthCheckOptions()
    {
        Predicate = e => e.Tags.Contains("API"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        ResultStatusCodes =
            {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status406NotAcceptable,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
    });

});

app.MapHealthChecksUI();

app.MapControllers();

app.Run();

