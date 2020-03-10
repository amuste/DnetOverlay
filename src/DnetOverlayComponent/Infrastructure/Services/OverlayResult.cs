using DnetOverlayComponent.Infrastructure.Enums;

namespace DnetOverlayComponent.Infrastructure.Services
{
    public class OverlayResult
    {

        public CloseReason CloseReason { get; set; }

        public object ComponentData { get; set; }

        public int OverlayRef { get; set; }
    }
}
