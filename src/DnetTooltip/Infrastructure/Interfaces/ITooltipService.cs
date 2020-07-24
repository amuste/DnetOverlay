using System.Threading.Tasks;
using DnetOverlayComponent.Infrastructure.Models;
using DnetOverlayComponent.Infrastructure.Services;
using DnetTooltip.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace DnetTooltip.Infrastructure.Interfaces
{
    public interface ITooltipService
    {
        OverlayReference Show(TooltipConfig tooltipConfig, ElementReference elementReference);

        OverlayReference Show<TComponent>(TooltipConfig tooltipConfig, ElementReference elementReference) where TComponent : ComponentBase;

        void Close(OverlayResult overlayDataResult);
    }
}
