using System;
using System.Threading.Tasks;
using DnetOverlayComponent.Infrastructure.Models;

namespace DnetOverlayComponent.Infrastructure.Interfaces
{
    public interface IViewportRuler
    {
        Task<Size> GetViewportSize();

        Task<ClientRect> GetViewportRect();

        Task<ViewportScrollPosition> GetViewportScrollPosition();

        Task<ClientRect> GetViewportRectNoScroll(int viewportMargin);

        void OnWindowResized(Size size);

        event EventHandler<Size> OnResized;

        ValueTask<bool> AddWindowEventListeners();

        ValueTask RemoveWindowEventListeners();

    }
}
