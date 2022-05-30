using Elders.Cronus.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace Elders.Cronus.Dashboard.Components
{
    public class ReplayPublicEventBase : ComponentBase
    {
        [Inject]
        protected AppState App { get; set; }

        [Inject]
        protected TokenClient Token { get; set; }

        [Inject]
        protected CronusClient Cronus { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        public ILogger<ConnectionsBase> Logger { get; set; }

        [Parameter]
        public bool? HasValidToken { get; set; }

        [Parameter]
        public Connection Connection { get; set; }

        [Parameter]
        public List<string> LiveTenants { get; set; }

        [Parameter]
        public List<string> LiveBoundedContexts { get; set; }

        protected List<DomainEventDto> Events { get; set; }

        protected ReplayPublicEventValidator validationModel = new ReplayPublicEventValidator();

        protected override async Task OnInitializedAsync()
        {
            App.OnTenantChanged += OnTenantChange;
            HasValidToken = await IsTokenValid();
            Connection = App.Connection;

            Events = new List<DomainEventDto>();

            Task<DomainDto> domainTask = Cronus.GetDomainAsync(@App.Connection);
            Task<List<string>> servicesTask = Cronus.GetLiveServicesAsync(Connection);
            Task<List<string>> tenantsTask = Cronus.GetLiveTenantsAsync(Connection);

            await Task.WhenAll(domainTask, servicesTask, tenantsTask);

            const string onlyPublic = "_Public";

            DomainDto domainResult = domainTask.Result;
            Events.AddRange(domainTask.Result.Events.Where(x => x.Name.Contains(onlyPublic)).Distinct());

            StateHasChanged();
        }

        protected async Task OnTenantChange(oAuth oAuth)
        {
            HasValidToken = await IsTokenValid();
            StateHasChanged();
        }

        protected async Task Success()
        {
            DateTimeOffset? replayAfter = null;
            if (validationModel.ReplayAfter.HasValue)
                replayAfter = new DateTimeOffset(validationModel.ReplayAfter.Value);

            var model = new ReplayPublicEventRequest()
            {
                Tenant = validationModel.LiveTenant,
                RecipientBoundedContext = validationModel.LiveBoundedContext,
                RecipientHandlers = validationModel.RecipientHandler,
                SourceEventTypeId = validationModel.SourceEventTypeId,
                ReplayAfter = replayAfter
            };

            bool result = await Cronus.ReplayPublicEventAsync(App.Connection, model);
            if (result == true)
                await JSRuntime.InvokeAsync<object>("alert", "Signal has been successfully sent");
            else
                await JSRuntime.InvokeAsync<object>("alert", "The signal could not be sent successfully");
        }


        protected void Reset()
        {
            validationModel.LiveTenant = null;
            validationModel.LiveBoundedContext = null;
            validationModel.RecipientHandler = null;
            validationModel.SourceEventTypeId = null;
        }

        private async Task<bool> IsTokenValid()
        {
            string result = await Token.GetAccessTokenAsync(App.Connection);
            if (string.IsNullOrEmpty(result))
                return false;

            return true;
        }

        protected class ReplayPublicEventValidator
        {
            [Required]
            public string LiveTenant { get; set; }

            [Required]
            public string LiveBoundedContext { get; set; }

            [Required]
            public string RecipientHandler { get; set; }

            [Required]
            public string SourceEventTypeId { get; set; }

            public DateTime? ReplayAfter { get; set; }
        }
    }
}
