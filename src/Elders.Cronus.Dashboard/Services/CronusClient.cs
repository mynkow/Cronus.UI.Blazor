using Microsoft.AspNetCore.Components;

namespace Elders.Cronus.Dashboard.Models
{
    public class CronusClient : HttpClientBase
    {
        private readonly TokenClient token;
        private readonly ILogger<CronusClient> log;
        [Inject]
        public NavigationManager NavManager { get; set; }
        public CronusClient(HttpClient client, TokenClient token, ILogger<CronusClient> log) : base(client)
        {
            this.token = token;
            this.log = log;
        }

        public async Task<List<string>> GetTenantsAsync(Connection connection)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/domain/tenants");
                if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
                {
                    var accessToken = await token.GetAccessTokenAsync(connection);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
                }

                var response = await ExecuteRequestAsync<List<string>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                log.LogInformation($"Probably the domain for Cronus Client is not the right one. s{ex.Message}");
                return new List<string>();
            }
        }

        public async Task<Response<ProjectionCollection>> GetProjectionsAsync(Connection connection)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/projections");
            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false && string.IsNullOrEmpty(connection.oAuth.Tenant) == false)
            {

                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            var response = await ExecuteRequestAsync<Response<ProjectionCollection>>(request);

            return response.Data;
        }

        public async Task<Response<IndexCollection>> GetIndicesAsync(Connection connection)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/indices"))
            {
                if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
                {
                    var accessToken = await token.GetAccessTokenAsync(connection);
                    log.LogInformation(accessToken);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    foreach (var header in request.Headers)
                    {
                        log.LogInformation(header.Key + ": " + string.Join(' ', header.Value));
                    }
                }

                var response = await ExecuteRequestAsync<Response<IndexCollection>>(request);

                return response.Data;
            }
        }

        public async Task<DomainDto> GetDomainAsync(Connection connection)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/domain/explore");
            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            var response = await ExecuteRequestAsync<DomainDto>(request);

            return response.Data;
        }

        public async Task<bool> RebuildProjectionAsync(Connection connection, Projection projection)
        {
            log.LogInformation("Rebuilding...");

            string resource = connection.CronusEndpoint + "/projection/rebuild";

            var rebuildRequest = new RebuildRequest()
            {
                ProjectionContractId = projection.ProjectionContractId,
                Hash = projection.LatestVersion.Hash
            };

            HttpRequestMessage request = CreateJsonPostRequest(rebuildRequest, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);
            return true;
        }

        public async Task<bool> ReplayProjectionAsync(Connection connection, Projection projection)
        {
            log.LogInformation("Replaying...");

            string resource = connection.CronusEndpoint + "/projection/replay";

            var rebuildRequest = new RebuildRequest()
            {
                ProjectionContractId = projection.ProjectionContractId,
                Hash = projection.LatestVersion.Hash
            };

            HttpRequestMessage request = CreateJsonPostRequest(rebuildRequest, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);


            return true;
        }

        /// <summary>
        /// This was the way we canceled projection versions before. It would cancel the latest version.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public async Task<bool> CancelProjectionLatestVersionAsync(Connection connection, Projection projection)
        {
            return await CancelProjectionAsync(connection, projection, projection.LatestVersion);
        }

        public async Task<bool> CancelSpecificProjectionAsync(Connection connection, Projection projection, ProjectionVersion version)
        {
            return await CancelProjectionAsync(connection, projection, version);
        }

        public async Task<bool> FinalizeIndexRebuildAsync(Connection connection, string indexContractId)
        {
            log.LogInformation("Finalizing index request...");

            string resource = connection.CronusEndpoint + "/index/finalize";

            var rebuildRequest = new IndexFinalizeRebuildRequestManually()
            {
                IndexContractId = indexContractId
            };

            HttpRequestMessage request = CreateJsonPostRequest(rebuildRequest, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);

            return true;
        }

        public async Task<bool> RebuildIndexAsync(Connection connection, string indexContractId)
        {
            log.LogInformation("Rebuild index!");

            string resource = connection.CronusEndpoint + "/EventStore/Index/Rebuild";

            var rebuildRequest = new RebuildIndex()
            {
                Id = indexContractId
            };

            HttpRequestMessage request = CreateJsonPostRequest(rebuildRequest, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);

            return true;
        }

        public async Task<bool> ReplayPublicEventAsync(Connection connection, ReplayPublicEventRequest model)
        {
            log.LogInformation($"Replay public event {model.SourceEventTypeId}");

            string resource = connection.CronusEndpoint + "/ReplayPublicEvent";

            HttpRequestMessage request = CreateJsonPostRequest(model, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);

            return true;
        }

        public async Task<List<string>> GetLiveServicesAsync(Connection connection)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/GetLiveBoundedContexts");
                if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
                {
                    var accessToken = await token.GetAccessTokenAsync(connection);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
                }

                var response = await ExecuteRequestAsync<List<string>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                log.LogInformation($"Probably the domain for Cronus Client is not the right one. s{ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetLiveTenantsAsync(Connection connection)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + "/GetTenantContexts");
                if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
                {
                    var accessToken = await token.GetAccessTokenAsync(connection);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
                }

                var response = await ExecuteRequestAsync<List<string>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                log.LogInformation($"Probably the domain for Cronus Client is not the right one. s{ex.Message}");
                return new List<string>();
            }
        }

        public async Task<AggregateDto> GetAggregate(Connection connection, string aggregateId)
        {
            if (string.IsNullOrEmpty(aggregateId)) throw new ArgumentNullException(nameof(aggregateId));
            log.LogDebug($"GetAggregate({aggregateId})");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + $"/EventStore/Explore?id={aggregateId}");
            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            var result = await ExecuteRequestAsync<Response<AggregateDto>>(request);

            return result.Data.Result;
        }

        public async Task RepublishEventAsync(Connection connection, string aggregateId, int commitRevision, int eventPosition, string[] recipientHandlers, bool isPublicEvent)
        {
            log.LogInformation("Republishing event request...");

            string resource = connection.CronusEndpoint + "/EventStore/Republish";

            var requestModel = new RepublishEventRequest()
            {
                Id = aggregateId,
                CommitRevision = commitRevision,
                EventPosition = eventPosition,
                RecipientHandlers = recipientHandlers,
                IsPublicEvent = isPublicEvent
            };

            HttpRequestMessage request = CreateJsonPostRequest(requestModel, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);
        }

        public async Task<ProjectionStateDto> GetProjectionAsync(Connection connection, string projectionName, string projectionId)
        {
            if (string.IsNullOrEmpty(projectionName)) throw new ArgumentNullException(nameof(projectionName));
            if (string.IsNullOrEmpty(projectionId)) throw new ArgumentNullException(nameof(projectionId));

            log.LogDebug($"{projectionName}({projectionId})");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + $"/Projection/Explore?projectionName={projectionName}&id={projectionId}");
            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            var result = await ExecuteRequestAsync<Response<ProjectionStateDto>>(request);

            return result.Data.Result;
        }

        public async Task<ProjectionCommitsDto> GetProjectionEventsAsync(Connection connection, string projectionName, string projectionId)
        {
            if (string.IsNullOrEmpty(projectionName)) throw new ArgumentNullException(nameof(projectionName));
            if (string.IsNullOrEmpty(projectionId)) throw new ArgumentNullException(nameof(projectionId));

            log.LogDebug($"{projectionName}({projectionId})");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, connection.CronusEndpoint + $"/Projection/ExploreEvents?projectionName={projectionName}&id={projectionId}");
            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            var result = await ExecuteRequestAsync<Response<ProjectionCommitsDto>>(request);

            return result.Data.Result;
        }

        private async Task<bool> CancelProjectionAsync(Connection connection, Projection projection, ProjectionVersion version)
        {
            log.LogInformation($"Canceling... {version}");

            string resource = connection.CronusEndpoint + "/projection/cancel";

            var rebuildRequest = new CancelProjectionRebuildRequest()
            {
                ProjectionContractId = projection.ProjectionContractId,
                Version = version,

            };

            HttpRequestMessage request = CreateJsonPostRequest(rebuildRequest, resource);

            if (string.IsNullOrEmpty(connection.oAuth.ServerEndpoint) == false)
            {
                var accessToken = await token.GetAccessTokenAsync(connection);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            }

            await ExecuteRequestAsync<object>(request);

            return true;
        }
    }

    public class DomainDto
    {
        public List<DomainAggregateDto> Aggregates { get; set; }

        public List<DomainGatewayDto> Gateways { get; set; }

        public List<DomainProjectionDto> Projections { get; set; }

        public List<DomainPortDto> Ports { get; set; }

        public List<DomainSagaDto> Sagas { get; set; }

        public IEnumerable<IMessageHandlerDto> FindHandlers(IMessageDto message)
        {
            return
                Sagas.Where(x => x.Events.Any(@event => @event.Id == message.Id)).Cast<IMessageHandlerDto>().Concat(
                Projections.Where(x => x.Events.Any(@event => @event.Id == message.Id))).Cast<IMessageHandlerDto>().Concat(
                Ports.Where(x => x.Events.Any(@event => @event.Id == message.Id))).Cast<IMessageHandlerDto>().Concat(
                Gateways.Where(x => x.Events.Any(@event => @event.Id == message.Id))).Cast<IMessageHandlerDto>();
        }
    }

    public interface IMessageDto
    {
        string Id { get; set; }
        string Name { get; set; }
    }

    public interface IMessageHandlerDto
    {
        string Id { get; set; }
        string Name { get; set; }
        string Type { get; set; }
    }

    public class DomainAggregateDto
    {
        public DomainAggregateDto()
        {
            Commands = new List<DomainCommandDto>();
            Events = new List<DomainEventDto>();
        }

        public string Name { get; set; }

        public List<DomainCommandDto> Commands { get; set; }
        public List<DomainEventDto> Events { get; set; }
    }

    public class DomainEventDto : IMessageDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DomainPortDto : IMessageHandlerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } = "Port";

        public List<DomainEventDto> Events { get; set; }
    }

    public class DomainGatewayDto : IMessageHandlerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } = "Gateway";

        public List<DomainEventDto> Events { get; set; }
    }

    public class DomainSagaDto : IMessageHandlerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } = "Saga";

        public List<DomainEventDto> Events { get; set; }
    }

    public class DomainCommandDto : IMessageDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DomainProjectionDto : IMessageHandlerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } = "Projection";

        public List<DomainEventDto> Events { get; set; }
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

    public class ProjectionCommitsDto
    {
        public string Name { get; set; }
        public List<ProjectionCommitDto> Commits { get; set; }
    }

    public class ProjectionCommitDto
    {
        public List<EventDto> Events { get; set; }

        public DateTimeOffset Timestamp { get; set; }
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
        public string Id { get; set; }

        public string EventName { get; set; }

        public object EventData { get; set; }

        public bool IsEntityEvent { get; set; }

        public bool IsPublicEvent { get; set; }

        public int EventPosition { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }

    public class RebuildRequest
    {
        public string ProjectionContractId { get; set; }

        public string Hash { get; set; }
    }

    public class CancelProjectionRebuildRequest
    {
        public string ProjectionContractId { get; set; }

        public ProjectionVersion Version { get; set; }

        public string Reason { get; set; } = "Cause I can";
    }

    public class IndexFinalizeRebuildRequestManually
    {
        public string IndexContractId { get; set; }
    }

    public class RebuildIndex
    {
        public string Id { get; set; }
    }

    public class RepublishEventRequest
    {
        public string Id { get; set; }

        public int CommitRevision { get; set; }

        public int EventPosition { get; set; }

        public string[] RecipientHandlers { get; set; }

        public bool IsPublicEvent { get; set; }
    }

    public class ReplayPublicEventRequest
    {
        public string LiveTenant { get; set; }

        public string LiveBoundedContext { get; set; }

        public string RecipientHandlers { get; set; }

        public string SourceEventTypeId { get; set; }

        public DateTimeOffset? ReplayAfter { get; set; }
    }

    public class Response<T>
    {
        public T Result { get; set; }

        public string Errors { get; set; }

        public bool IsSuccess { get; set; }
    }
}
