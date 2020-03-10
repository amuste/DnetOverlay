using DnetOverlayComponent.Infrastructure.Enums;

namespace DnetOverlayComponent.Infrastructure.Models
{
    public class ConnectionPositionPair
    {
        public HorizontalConnectionPos OriginX { get; set; }

        public VerticalConnectionPos OriginY { get; set; }

        public HorizontalConnectionPos OverlayX { get; set; }

        public VerticalConnectionPos OverlayY { get; set; }
    }
}
