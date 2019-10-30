using System;
using Microsoft.AspNetCore.Components;

namespace Elders.Cronus.Dashboard.Components
{
    public class DialogBase : ComponentBase
    {
        private bool isVisible;

        [Parameter]
        public bool IsVisible
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
        public Action<bool> IsVisibleChanged { get; set; }
    }
}
