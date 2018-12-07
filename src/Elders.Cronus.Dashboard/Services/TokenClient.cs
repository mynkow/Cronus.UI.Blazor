using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Elders.Cronus.Dashboard.Models
{
    public class TokenClient
    {
        private readonly HttpClient httpClient;

        public TokenClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(Connection connection)
        {
            HttpRequestMessage getTokenRequest = new HttpRequestMessage(HttpMethod.Post, connection.oAuth.ServerEndpoint);
            getTokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", connection.oAuth.BasicAuthorization);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("scope", "read");
            getTokenRequest.Content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.SendAsync(getTokenRequest);
            var result = await response.Content.ReadAsStringAsync();
            var obj = Microsoft.JSInterop.Json.Deserialize<TokenResult>(result);
            return obj.Access_Token;
        }

        public class TokenResult
        {
            public string Access_Token { get; set; }

            public long Expires_in { get; set; }

            public string Token_type { get; set; }
        }
    }


}
