namespace DnetDialogComponent.Infrastructure.Models
{
    public class ContentOptions<TItem>
    {
        public int OverlayRef { get; set; }

        public TItem Options { get; set; }
    }
}
