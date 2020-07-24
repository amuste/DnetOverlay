using DnetConnectedPanel.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetConnectedPanel.Infrastructure.Interfaces
{
    public interface IConnectedPanelService
    {
        OverlayReference Open<TComponent, TComponentOptions>(ConnectedPanelConfig overlayConfig, ContentData componentOptions) where TComponent : ComponentBase;

        void Close(OverlayResult overlayDataResult);
    }
}
