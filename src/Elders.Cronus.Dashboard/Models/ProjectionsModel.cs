using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Elders.Cronus.Dashboard.Models
{
    public class CronusClient
    {
        private readonly HttpClient client;
        private readonly ILogger<HttpClient> log;

        public CronusClient(HttpClient client, ILogger<HttpClient> log)
        {
            this.client = client;
            this.log = log;
        }

        public async Task<Response<ProjectionsResult>> GetProjectionsAsync(Connection connection)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpiont + "/projections");
            var accessToken = await connection.oAuth.GetAccessTokenAsync(client);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);
            var obj = Json.Deserialize<Response<ProjectionsResult>>(result);

            foreach (var projection in obj.Result.Projections)
            {
                string versions = string.Join(" | ", projection.Versions);
                log.LogDebug($"{projection.ProjectionName}: {versions}");
            }

            return obj;
        }

        public async Task<bool> RebuildAsync(Connection connection, Projection projection)
        {
            log.LogInformation("Rebuilding...");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, connection.CronusEndpiont + "/projection/rebuild");
            var accessToken = await connection.oAuth.GetAccessTokenAsync(client);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var rebuildRequest = new RebuildRequest()
            {
                ProjectionContractId = projection.ProjectionContractId,
                Hash = projection.LatestVersion.Hash
            };

            request.Content = new StringContent(Json.Serialize(rebuildRequest), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);

            return true;
        }
    }

    public class RebuildRequest
    {
        public string ProjectionContractId { get; set; }

        public string Hash { get; set; }
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

        public string ProjectionContractId { get; set; }

        public string ProjectionName { get; set; }

        public List<ProjectionVersion> Versions { get; set; }

        public ProjectionVersion LiveVersion => Versions.Where(x => x.Status.Equals(ProjectionStatus.Live)).SingleOrDefault();

        public ProjectionVersion LatestVersion => Versions.OrderByDescending(x => x.Revision).First();
    }

    public class ProjectionVersion
    {
        public string Hash { get; set; }

        public int Revision { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return $"{Status}-{Hash}-{Revision}";
        }
    }

    public class ProjectionStatus
    {
        public const string Live = "live";
        public const string Building = "building";
        public const string NotPresent = "not_present";
        public const string Timedout = "timedout";
        public const string Canceled = "canceled";
    }
}
