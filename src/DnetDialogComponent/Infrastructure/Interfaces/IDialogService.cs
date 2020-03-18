using DnetDialogComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace DnetDialogComponent.Infrastructure.Interfaces
{
    public interface IDialogService
    {
        OverlayReference Open<TComponent, TComponentOptions>(DialogConfig overlayConfig, ContentData componentOptions) where TComponent : ComponentBase;

        void Close(OverlayResult overlayDataResult);
    }
}
