using System;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using DnetToastComponent.Infrastructure.Enums;
using DnetToastComponent.Infrastructure.Interfaces;
using DnetToastComponent.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetToastComponent.Infrastructure.Services
{
    public class ToastService : IToastService
    {
        private readonly IOverlayService _overlayService;

        private int _toastCounter = 0;

        public ToastService(IOverlayService overlayService)
        {
            _overlayService = overlayService;
        }

        public void Show(ToastConfig overlayConfig)
        {
            Open(null, overlayConfig);
        }

        public void Show<TComponent>(ToastConfig overlayConfig) where TComponent : ComponentBase
        {
            Open(typeof(TComponent), overlayConfig);
        }

        private void Open(Type componentType, ToastConfig toastConfig)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(componentType) && componentType != null)
            {
                throw new ArgumentException($"{componentType.FullName} must be a Blazor Component");
            }

            var globalPositionStrategy = new GlobalPositionStrategyBuilder();

            var offsetBottom = toastConfig.OffsetBottom > 0 ? toastConfig.OffsetBottom : null;

            var offsetRight = toastConfig.OffsetRight > 0 ? toastConfig.OffsetRight : null;

            var offsetTop = toastConfig.OffsetTop > 0 ? toastConfig.OffsetTop : null;

            var offsetLeft = toastConfig.OffsetLeft > 0 ? toastConfig.OffsetLeft : null;

            switch (toastConfig.ToastPostion)
            {
                case ToastPostion.BottomCenter:

                    globalPositionStrategy.Bottom($"{offsetBottom + (80 * _toastCounter)}px");
                    globalPositionStrategy.CenterHorizontally("");

                    break;

                case ToastPostion.BottomRight:

                    globalPositionStrategy.Bottom($"{offsetBottom + (80 * _toastCounter)}px");
                    globalPositionStrategy.Right(offsetRight + "px");

                    break;

                case ToastPostion.BottomLeft:

                    globalPositionStrategy.Bottom(offsetBottom + "px");
                    globalPositionStrategy.Left(offsetLeft + "px");

                    break;

                case ToastPostion.TopCenter:

                    globalPositionStrategy.Top(offsetTop + "px");
                    globalPositionStrategy.CenterHorizontally("");

                    break;

                case ToastPostion.TopRight:

                    globalPositionStrategy.Top($"{offsetTop + (80 * _toastCounter)}px");
                    globalPositionStrategy.Right(offsetRight + "px");

                    break;

                case ToastPostion.TopLeft:

                    globalPositionStrategy.Top($"{offsetTop + (80 * _toastCounter)}px");
                    globalPositionStrategy.Left(offsetLeft + "px");

                    break;
                case ToastPostion.LeftCenter:

                    globalPositionStrategy.Left(offsetLeft + "px");
                    globalPositionStrategy.CenterVertically("");

                    break;

                case ToastPostion.RightCenter:

                    globalPositionStrategy.Right(offsetRight + "px");
                    globalPositionStrategy.CenterVertically("");

                    break;
            }

            var overlayConfig = new OverlayConfig()
            {
                HasBackdrop = toastConfig.HasBackdrop,
                HasTransparentBackdrop = toastConfig.HasTransparentBackdrop,
                Width = toastConfig.Width + "px",
                Height = toastConfig.Height + "px",
                GlobalPositionStrategy = globalPositionStrategy,
                MaxHeight = "170px"
            };

            var userContent = new RenderFragment(x => {});

            if (componentType != null)
            {
                userContent = x =>
                {
                    x.OpenComponent(0, componentType);
                    x.AddAttribute(1, "ContentData", toastConfig.Text);
                    x.CloseComponent();
                };
            }

            var toast = new RenderFragment(x =>
            {
                x.OpenComponent(0, typeof(DnetToast));
                x.AddAttribute(1, "Title", toastConfig.Title);
                x.AddAttribute(2, "ToastClass", toastConfig.ToastClass);
                if (componentType != null) x.AddAttribute(3, "ContentChild", userContent);
                x.AddAttribute(4, "Text", toastConfig.Text);
                x.AddAttribute(5, "ToastType", toastConfig.ToastType);
                x.AddAttribute(6, "ToastTypeIconClass", toastConfig.ToastTypeIconClass);
                x.AddAttribute(7, "TypeIconClass", toastConfig.ToastTypeColor);
                x.AddAttribute(8, "ExcutionTime", toastConfig.ExcutionTime);
                x.AddAttribute(9, "ShowExcutionTime", toastConfig.ShowExcutionTime);
                x.CloseComponent();
            });

            _overlayService.Attach(toast, overlayConfig);

            _toastCounter++;
        }

        public void Close(OverlayResult overlayDataResult)
        {
            _toastCounter--;
            if (_toastCounter < 0) _toastCounter = 0;
            _overlayService.Detach(overlayDataResult);
        }
    }
}
