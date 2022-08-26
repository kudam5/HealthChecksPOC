using System.Net.Http.Headers;

namespace HealthMonitorPOC.Web.APIHelper
{
    public interface IAPIService
    {
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> GetAsync(Uri uri, AuthenticationHeaderValue?  authenticationHeaderValue);
        Task<HttpResponseMessage> PostAsync(Uri uri, AuthenticationHeaderValue? authenticationHeaderValue, string content);
    }
}
