using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace Elders.Cronus.Dashboard.Models
{
    public class LSKey
    {
        public const string Connections = "connections";
    }

    public class Connection
    {
        public string Name { get; set; }

        public string CronusEndpiont { get; set; }

        public oAuth oAuth { get; set; }
    }

    public class oAuth
    {
        public string ServerEndpoint { get; set; }

        public string Client { get; set; }

        public string Secret { get; set; }

        public async Task<string> GetAccessTokenAsync(HttpClient client)
        {
            HttpRequestMessage getTokenRequest = new HttpRequestMessage(HttpMethod.Post, ServerEndpoint);
            getTokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Client}:{Secret}")));
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("scope", "read");
            getTokenRequest.Content = new FormUrlEncodedContent(parameters);
            var response = await client.SendAsync(getTokenRequest);
            var result = await response.Content.ReadAsStringAsync();
            var obj = Microsoft.JSInterop.Json.Deserialize<TokenResult>(result);
            return obj.Access_Token;
        }
    }

    public class TokenResult
    {
        public string Access_Token { get; set; }

        public long Expires_in { get; set; }

        public string Token_type { get; set; }
    }
}
