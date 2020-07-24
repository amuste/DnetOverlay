using System;
using DnetConnectedPanel.Infrastructure.Interfaces;
using DnetConnectedPanel.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetConnectedPanel.Infrastructure.Services
{
    public class ConnectedPanelService : IConnectedPanelService
    {
        private readonly IOverlayService _overlayService;

        public ConnectedPanelService(IOverlayService overlayService)
        {
            _overlayService = overlayService;
        }

        public OverlayReference Open<TComponent, TContentData>(ConnectedPanelConfig connectedPanelConfig, ContentData componentOptions) where TComponent : ComponentBase
        {
            var reference = Open<TContentData>(typeof(TComponent), connectedPanelConfig, componentOptions);

            return reference;
        }

        private OverlayReference Open<TContentData>(Type componentType, ConnectedPanelConfig connectedPanelConfig, ContentData contentData)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(componentType))
            {
                throw new ArgumentException($"{componentType.FullName} must be a Blazor Component");
            }

            var globalPositionStrategy = new GlobalPositionStrategyBuilder();

            globalPositionStrategy.CenterVertically(null);
            globalPositionStrategy.CenterHorizontally(null);

            var overlayConfig = new OverlayConfig()
            {
                HasBackdrop = connectedPanelConfig.HasBackdrop,
                HasTransparentBackdrop = connectedPanelConfig.HasTransparentBackdrop,
                Width = connectedPanelConfig.Width,
                Height = connectedPanelConfig.Height,
                GlobalPositionStrategy = globalPositionStrategy
            };

            var userContent = new RenderFragment(x =>
            {
                x.OpenComponent(0, componentType);
                x.AddAttribute(1, "ContentData", contentData);
                x.CloseComponent();
            });

            var dialog = new RenderFragment(x =>
            {
                x.OpenComponent(0, typeof(DnetConnectedFloatingPanel));
                x.AddAttribute(1, "Title", connectedPanelConfig.Title);
                x.AddAttribute(2, "DialogClass", connectedPanelConfig.DialogClass);
                x.AddAttribute(3, "ContentChild", userContent);
                x.CloseComponent();
            });

            var overlayReference = _overlayService.Attach(dialog, overlayConfig);

            return overlayReference;
        }

        public void Close(OverlayResult overlayDataResult)
        {
            _overlayService.Detach(overlayDataResult);
        }
    }
}
