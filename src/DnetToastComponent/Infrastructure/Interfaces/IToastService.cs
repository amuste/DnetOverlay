using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using DnetToastComponent.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetToastComponent.Infrastructure.Interfaces
{
    public interface IToastService
    {
        void Show(ToastConfig overlayConfig);

        void Show<TComponent>(ToastConfig overlayConfig) where TComponent : ComponentBase;

        void Close(OverlayResult overlayDataResult);
    }
}
