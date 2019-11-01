using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Elders.Cronus.Dashboard.Models
{
    public class CronusClient
    {
        private readonly HttpClient client;
        private readonly TokenClient token;
        private readonly ILogger<CronusClient> log;
        private readonly JsonSerializerOptions options;

        public CronusClient(HttpClient client, TokenClient token, ILogger<CronusClient> log)
        {
            this.client = client;
            this.token = token;
            this.log = log;
            this.options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<Response<ProjectionCollection>> GetProjectionsAsync(Connection connection)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/projections");
            var accessToken = await token.GetAccessTokenAsync(connection);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);
            var obj = JsonSerializer.Deserialize<Response<ProjectionCollection>>(result, options);

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

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, connection.CronusEndpoint + "/projection/rebuild");
            var accessToken = await token.GetAccessTokenAsync(connection);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var rebuildRequest = new RebuildRequest()
            {
                ProjectionContractId = projection.ProjectionContractId,
                Hash = projection.LatestVersion.Hash
            };

            request.Content = new StringContent(JsonSerializer.Serialize(rebuildRequest), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);

            return true;
        }

        public async Task<AggregateDto> GetAggregate(Connection connection, string aggregateId)
        {
            if (string.IsNullOrEmpty(aggregateId)) throw new ArgumentNullException(nameof(aggregateId));
            log.LogDebug($"GetAggregate({aggregateId})");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + $"/EventStore/Explore?id={aggregateId}");
            var accessToken = await token.GetAccessTokenAsync(connection);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);
            var obj = JsonSerializer.Deserialize<Response<AggregateDto>>(result, options);
            
            return obj.Result;
        }

        public async Task<ProjectionStateDto> GetProjectionAsync(Connection connection, string projectionName, string projectionId)
        {
            if (string.IsNullOrEmpty(projectionName)) throw new ArgumentNullException(nameof(projectionName));
            if (string.IsNullOrEmpty(projectionId)) throw new ArgumentNullException(nameof(projectionId));

            log.LogDebug($"{projectionName}({projectionId})");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + $"/Projection/Explore?projectionName={projectionName}&id={projectionId}");
            var accessToken = await token.GetAccessTokenAsync(connection);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            log.LogDebug(result);
            var obj = JsonSerializer.Deserialize<Response<ProjectionStateDto>>(result, options);

            return obj.Result;
        }
    }

    public class AggregateDto
    {
        public AggregateDto()
        {
            Commits = new List<AggregateCommitDto>();
        }

        public string BoundedContext { get; set; }

        public string AggregateId { get; set; }

        public List<AggregateCommitDto> Commits { get; set; }
    }

    public class ProjectionStateDto
    {
        public string Name { get; set; }
        public object State { get; set; }
    }

    public class AggregateCommitDto
    {
        public AggregateCommitDto()
        {
            Events = new List<EventDto>();
        }

        public int AggregateRootRevision { get; set; }

        public List<EventDto> Events { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public class EventDto
    {
        public string EventName { get; set; }

        public object EventData { get; set; }
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
}
