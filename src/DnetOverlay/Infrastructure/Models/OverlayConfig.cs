﻿using DnetOverlay.Infrastructure.Enums;

namespace DnetOverlay.Infrastructure.Models
{
    public class OverlayConfig
    {
        public string PanelClass { get; set; } = null;

        public bool HasBackdrop { get; set; } = true;

        public bool HasTransparentBackdrop { get; set; }

        public string BackdropClass { get; set; } = null;

        public string Width { get; set; } = "100%";

        public string Height { get; set; } = "100%";

        public string MinWidth { get; set; } = null;

        public string MinHeight { get; set; } = null;

        public string MaxWidth { get; set; } = null;

        public string MaxHeight { get; set; } = null;

        public PositionStrategy PositionStrategy { get; set; } = PositionStrategy.Global;

        public GlobalPositionStrategy GlobalPositionStrategy { get; set; }
    }
}