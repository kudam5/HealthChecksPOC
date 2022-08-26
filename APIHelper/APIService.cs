using System.Net.Http.Headers;
using System.Text;

namespace HealthMonitorPOC.Web.APIHelper
{
    public class APIService : IAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public APIService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            var httpClient = SetupHttpClient(uri, null);

            var response = await httpClient.GetAsync(uri.ToString()).ConfigureAwait(false);
            return await Task.FromResult(response);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri, AuthenticationHeaderValue? authenticationHeaderValue)
        {
            var httpClient = SetupHttpClient(uri, authenticationHeaderValue);

            var response = await httpClient.GetAsync(uri.ToString()).ConfigureAwait(false);
            return await Task.FromResult(response);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, AuthenticationHeaderValue? authenticationHeaderValue, string content)
        {
            var httpClient = SetupHttpClient(uri, authenticationHeaderValue);

            var data = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri.ToString(), data).ConfigureAwait(false);
            return await Task.FromResult(response);
        }

        private HttpClient SetupHttpClient(Uri uri, AuthenticationHeaderValue? authenticationHeaderValue)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string url = uri.ToString();
            httpClient.BaseAddress = new Uri(url.ToString());

            if (authenticationHeaderValue is not null)            
                httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;            

            return httpClient;
        }
    }
}
