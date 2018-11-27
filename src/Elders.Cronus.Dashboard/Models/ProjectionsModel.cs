using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Elders.Cronus.Dashboard.Models
{
    public class CronusClient
    {
        private readonly Connection connection;
        private readonly HttpClient client;
        private readonly ILogger<HttpClient> log;

        public CronusClient(Connection connection, HttpClient client, ILogger<HttpClient> log)
        {
            this.connection = connection;
            this.client = client;
            this.log = log;
        }

        public async Task<Response<ProjectionsResult>> GetProjections()
        {
            HttpRequestMessage getTokenRequest = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpiont + "/projections");
            var accessToken = await connection.oAuth.GetAccessTokenAsync(client);
            getTokenRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var response = await client.SendAsync(getTokenRequest);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);
            var obj = Json.Deserialize<Response<ProjectionsResult>>(result);

            return obj;
        }
    }

    public class Response<T>
    {
        public T Result { get; set; }

        public string Errors { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class ProjectionsResult
    {
        public ProjectionsResult()
        {
            Projections = new List<Projection>();
        }

        public List<Projection> Projections { get; set; }
    }

    public class Projection
    {
        public Projection()
        {
            Versions = new List<ProjectionVersion>();
        }

        public string projectionContractId { get; set; }

        public string projectionName { get; set; }

        public List<ProjectionVersion> Versions { get; set; }

        public bool IsLive => Versions.Where(x => x.Status.Equals(ProjectionStatus.Live)).Any();
    }

    public class ProjectionVersion
    {
        public string Hash { get; set; }

        public int Version { get; set; }

        public string Status { get; set; }
    }

    public class ProjectionStatus
    {
        public const string Live = "live";
        public const string Building = "building";
        public const string NotPresent = "building";
    }
}
