using Microsoft.AspNetCore.Blazor.Components;

namespace Elders.Cronus.Dashboard.Pages
{
    public class CounterBase : BlazorComponent
    {
        protected int currentCount = 0;

        protected void IncrementCount()
        {
            currentCount = currentCount + 5;
        }
    }
}
