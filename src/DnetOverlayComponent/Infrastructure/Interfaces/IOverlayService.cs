using System;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetOverlayComponent.Infrastructure.Interfaces
{
    public interface IOverlayService
    {
        event Action OnBackdropClicked;

        OverlayReference GetOverlayReference();

        OverlayReference Attach(RenderFragment overlayContent, OverlayConfig overlayConfig);

        void Detach(OverlayResult overlayDataResult);

        void BackdropClicked(OverlayResult overlayDataResult);
    }
}
