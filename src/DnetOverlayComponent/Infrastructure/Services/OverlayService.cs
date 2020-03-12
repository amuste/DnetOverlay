using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetOverlayComponent.Infrastructure.Services
{
    // Credit for this service to Chris Sainty
    // His Modal implementation point me in the right direction
    // https://github.com/Blazored/Modal
    public class OverlayService : IOverlayService
    {
        internal event Action<RenderFragment, OverlayConfig> OnAttach;

        public event Action<OverlayResult> OnDetach;

        public event Action OnBackdropClicked;

        private Type _componentType;

        private List<int> _sequenNumbers { get; set; } = new List<int>();

        private int _sequenceNumber { get; set; } = 1;

        public int Attach<T, TComponentOptions>(OverlayConfig overlayConfig, ComponentOptions<TComponentOptions> componentOptions) where T : ComponentBase
        {
            _sequenNumbers.Add(_sequenceNumber);

            overlayConfig.OverlayRef = _sequenceNumber;

            _sequenceNumber++;

            Attach<TComponentOptions>(typeof(T), overlayConfig, componentOptions);

            return overlayConfig.OverlayRef;
        }

        public void Attach<TComponentOptions>(Type contentComponent, OverlayConfig overlayConfig, ComponentOptions<TComponentOptions> componentOptions)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");
            }

            componentOptions.OverlayRef = overlayConfig.OverlayRef;

            var content = new RenderFragment(x =>
            {
                x.OpenComponent(0, contentComponent);
                x.AddAttribute(1, "ComponentOptions", componentOptions);
                x.CloseComponent();
            });
            _componentType = contentComponent;

            OnAttach?.Invoke(content, overlayConfig);
        }

        public void Detach(OverlayResult overlayDataResult)
        {
            _sequenNumbers.Remove(overlayDataResult.OverlayRef);

            if (!_sequenNumbers.Any()) _sequenceNumber = 1;

            OnDetach?.Invoke(overlayDataResult);
        }

        public void BackdropClicked()
        {
            OnBackdropClicked?.Invoke();
        }
    }
}
