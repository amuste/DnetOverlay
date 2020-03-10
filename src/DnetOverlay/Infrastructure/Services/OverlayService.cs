using System;
using DnetOverlay.Infrastructure.Interfaces;
using DnetOverlay.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetOverlay.Infrastructure.Services
{
    public class OverlayService : IOverlayService
    {
       
        //public event Action<string> OnClose;

        //internal event Action CloseModal;

        internal event Action<RenderFragment, OverlayConfig> OnShow;

        public event Action OnClose;

        private Type _modalType;

        public void Show<T>(OverlayConfig overlayConfig) where T : ComponentBase
        {
            Show(typeof(T), overlayConfig);
        }

       
        public void Show(Type contentComponent, OverlayConfig overlayConfig)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");
            }

            var content = new RenderFragment(x => { x.OpenComponent(1, contentComponent); x.CloseComponent(); });
            _modalType = contentComponent;

            OnShow?.Invoke(content, overlayConfig);
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}
