using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

namespace Elders.Cronus.Dashboard.Components.Bootstrap
{
    public class ModalBase : ComponentBase, System.IDisposable
    {
        [Inject]
        internal ILogger<ModalBase> Log { get; set; }

        [Inject]
        internal IJSRuntime JSRuntime { get; set; }

        public string Id { get; private set; } = "BM" + Guid.NewGuid().ToString().Replace("-", String.Empty);

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string PrimaryButtonText { get; set; }

        /// <summary>
        /// Ivoked when user clicks on primary button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        public Func<bool> PrimaryButtonClick { get; set; }

        [Parameter]
        public string CloseButtonText { get; set; } = "Close";

        /// <summary>
        /// Invoken when user clicks on close button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        public Func<bool> CloseButtonClick { get; set; }

        [Parameter]
        public Action<bool> IsVisibleChanged { get; set; }

        private bool isVisible;
        private bool isVisibleChanged;

        [Parameter]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                Log.LogDebug($"IsVisible: Current: {isVisible}, New: {value}.");
                if (isVisible != value)
                {
                    isVisible = value;

                    IsVisibleChanged?.Invoke(isVisible);
                    isVisibleChanged = true;
                }
            }
        }

        protected string DialogCssClass { get; set; }

        [Parameter]
        public ModalSize Size { get; set; } = ModalSize.Normal;

        [Parameter]
        public Action Closed { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            DialogCssClass = "modal-dialog";
            switch (Size)
            {
                case ModalSize.Small:
                    DialogCssClass += " modal-sm";
                    break;
                case ModalSize.Normal:
                    break;
                case ModalSize.Large:
                    DialogCssClass += " modal-lg";
                    break;
                default:
                    throw new NotSupportedException(Size.ToString());
            }
        }

        public void OnPrimaryButtonClick()
        {
            Log.LogDebug("Primary button click raised.");
            if (IsVisible)
            {
                //Log.Debug("Visibility constraint passed.");
                IsVisible = !PrimaryButtonClick();
            }
        }

        protected void OnFormSubmit(EventArgs e)
        {
            Log.LogDebug("Form onsubmit raised.");
            if (IsVisible)
            {
                Log.LogDebug("Visibility constraint passed.");
                IsVisible = !PrimaryButtonClick();
            }
        }

        protected void OnCloseButtonClick()
        {
            if (CloseButtonClick != null)
                IsVisible = !CloseButtonClick();
            else
                IsVisible = false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            Native.AddModal(Id, this);

            if (isVisibleChanged)
                JSRuntime.InvokeAsync<object>("Bootstrap.Modal.Toggle", Id, isVisible);
        }

        public void Dispose()
        {
            Native.RemoveModal(Id);
        }

        internal void MarkAsHidden()
        {
            IsVisible = false;
        }
    }

    public enum ModalSize
    {
        Small,
        Normal,
        Large
    }

    public class Native
    {
        [Inject]
        protected ILogger<Native> Log { get; set; }

        private static Dictionary<string, ModalBase> modals = new Dictionary<string, ModalBase>();

        internal static void AddModal(string id, ModalBase component)
        {
            modals[id] = component;
            component.JSRuntime.InvokeAsync<object>("Bootstrap.Modal.Register", id);
        }

        internal static void RemoveModal(string id)
            => modals.Remove(id);

        [JSInvokable]
        public static void Bootstrap_ModalHidden(string id)
        {
            //ILog log = Program.Resolve<ILogFactory>().Scope("Modal.Native");
            //log.Debug($"Modal hidden '{id}'.");
            if (modals.TryGetValue(id, out ModalBase modal))
                modal.MarkAsHidden();
            // else
            //log.Debug($"Modal not found '{id}'.");
        }
    }
}
