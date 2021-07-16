//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Blazored.LocalStorage;
//using Elders.Cronus.Dashboard.Models;
//using Microsoft.AspNetCore.Components;
//using Microsoft.Extensions.Logging;

//namespace Elders.Cronus.Dashboard.Pages
//{
//    public class DomainPortsBase : ComponentBase
//    {
//        [Inject]
//        public ILogger<DomainPortsBase> Logger { get; set; }

//        public DomainPortsBase()
//        {
//        }

//        protected override async Task OnInitializedAsync()
//        {
//        }

//        protected async Task OnGG(Connection model)
//        {
//            Logger.LogInformation("GG");

//            StateHasChanged();
//        }
//    }
//}
