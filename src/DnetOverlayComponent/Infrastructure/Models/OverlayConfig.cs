using System;
using DnetOverlayComponent.Infrastructure.Enums;

namespace DnetOverlayComponent.Infrastructure.Models
{
    public class OverlayConfig
    {
        public int OverlayRef { get; set; }

        public string PanelClass { get; set; } = null;

        public int? PanelZindex { get; set; } = null;

        public bool HasBackdrop { get; set; } = true;

        public int? BackdropZindex { get; set; } = null;

        public bool HasTransparentBackdrop { get; set; }

        public string BackdropClass { get; set; } = null;

        public string Width { get; set; } = null;

        public string Height { get; set; } = null;

        public string MinWidth { get; set; } = null;

        public string MinHeight { get; set; } = null;

        public string MaxWidth { get; set; } = null;

        public string MaxHeight { get; set; } = null;

        public PositionStrategy PositionStrategy { get; set; } = PositionStrategy.Global;

        public GlobalPositionStrategyBuilder GlobalPositionStrategy { get; set; }

        public FlexibleConnectedPositionStrategyBuilder FlexibleConnectedPositionStrategyBuilder { get; set; }
    }
}
