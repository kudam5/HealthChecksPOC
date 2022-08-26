using Confluent.Kafka;
using HealthMonitorPOC.Web.APIHelper;
using HealthMonitorPOC.Web.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HealthMonitorPOC.Web.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IAPIService _apiService;
        private readonly IOptions<KafkaSettings> _kafkaSettings;
        public KafkaProducer(IAPIService apiService, IOptions<KafkaSettings> kafkaSettings)
        {
            _apiService = apiService;
            _kafkaSettings = kafkaSettings;
        }
        
        public async Task<string> PublishEvent(PublishRequest request)
        {
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", _kafkaSettings.Value.ClusterAuthorization);

            var payload = JsonSerializer.Serialize(request);
            var result = await _apiService.PostAsync(new Uri(_kafkaSettings.Value.PublishEndpoint), authenticationHeaderValue, payload);
           
            var response = await result.Content.ReadAsStringAsync();
            return response;
        }

        public async Task<string> GetEvents()
        {
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", _kafkaSettings.Value.ClusterAuthorization);

            var result = await _apiService.GetAsync(new Uri(_kafkaSettings.Value.Endpoint), authenticationHeaderValue); ;

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return response;
            }

            return string.Empty;
        }
    }
}
