using System;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetOverlayComponent.Infrastructure.Interfaces
{
    public interface IOverlayService
    {
        int Attach<T, TComponentOptions>(OverlayConfig overlayConfig, ComponentOptions<TComponentOptions> componentOptions) where T : ComponentBase;

        void Attach<TComponentOptions>(Type contentComponent, OverlayConfig overlayConfig, ComponentOptions<TComponentOptions> componentOptions);

        void Detach(OverlayResult overlayDataResult);

        void BackdropClicked();
    }
}
