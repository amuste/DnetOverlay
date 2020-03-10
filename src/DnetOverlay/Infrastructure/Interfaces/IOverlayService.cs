using System;
using DnetOverlay.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetOverlay.Infrastructure.Interfaces
{
    public interface IOverlayService
    {
        void Show<T>(OverlayConfig overlayConfig) where T : ComponentBase;

        void Show(Type contentComponent, OverlayConfig overlayConfig);

        void Close();
    }
}
