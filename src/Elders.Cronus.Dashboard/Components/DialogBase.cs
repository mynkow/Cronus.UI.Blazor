using System;
using Microsoft.AspNetCore.Blazor.Components;

namespace Elders.Cronus.Dashboard.Components
{
    public class DialogBase : BlazorComponent
    {
        private bool isVisible;

        [Parameter]
        protected bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    IsVisibleChanged?.Invoke(isVisible);
                }
            }
        }

        [Parameter]
        protected Action<bool> IsVisibleChanged { get; set; }
    }
}
