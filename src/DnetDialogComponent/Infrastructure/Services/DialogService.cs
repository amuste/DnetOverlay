using System;
using DnetDialogComponent.Infrastructure.Interfaces;
using DnetDialogComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetDialogComponent.Infrastructure.Services
{
    public class DialogService : IDialogService
    {
        private readonly IOverlayService _overlayService;

        public DialogService(IOverlayService overlayService)
        {
            _overlayService = overlayService;
        }

        public OverlayReference Open<TComponent, TContentData>(DialogConfig overlayConfig, ContentData componentOptions) where TComponent : ComponentBase
        {
            var reference = Open<TContentData>(typeof(TComponent), overlayConfig, componentOptions);

            return reference;
        }

        private OverlayReference Open<TContentData>(Type componentType, DialogConfig dialogConfig, ContentData contentData)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(componentType))
            {
                throw new ArgumentException($"{componentType.FullName} must be a Blazor Component");
            }

            var globalPositionStrategy = new GlobalPositionStrategyBuilder();

            globalPositionStrategy.CenterVertically("");
            globalPositionStrategy.CenterHorizontally("");

            var overlayConfig = new OverlayConfig()
            {
                HasBackdrop = dialogConfig.HasBackdrop,
                HasTransparentBackdrop = dialogConfig.HasTransparentBackdrop,
                Width = dialogConfig.Width,
                Height = dialogConfig.Height,
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
                x.OpenComponent(0, typeof(DnetDialog));
                x.AddAttribute(1, "Title", dialogConfig.Title);
                x.AddAttribute(2, "DialogClass", dialogConfig.DialogClass);
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
