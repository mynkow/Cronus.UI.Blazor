using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Elders.Cronus.Dashboard.Models
{
    public class TokenClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TokenClient> log;

        public TokenClient(HttpClient httpClient, ILogger<TokenClient> log)
        {
            this.httpClient = httpClient;
            this.log = log;
        }

        public async Task<string> GetAccessTokenAsync(Connection connection)
        {
            HttpRequestMessage getTokenRequest = new HttpRequestMessage(HttpMethod.Post, connection.oAuth.ServerEndpoint);
            getTokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", connection.oAuth.BasicAuthorization);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");

            parameters.Add("scope", connection.oAuth.Scope);

            getTokenRequest.Content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.SendAsync(getTokenRequest).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var obj = JsonSerializer.Deserialize<TokenResult>(result);
            log.LogDebug(obj.access_token);
            return obj.access_token;
        }

        public class TokenResult
        {
            public string access_token { get; set; }

            public long expires_in { get; set; }

            public string token_type { get; set; }
        }
    }
}
