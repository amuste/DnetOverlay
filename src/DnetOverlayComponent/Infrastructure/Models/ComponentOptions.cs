using System;

namespace DnetOverlayComponent.Infrastructure.Models
{
    public class ComponentOptions<TItem>
    {
        public int OverlayRef { get; set; }

        public TItem Options { get; set; }
    }
}
