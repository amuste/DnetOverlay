using DnetConnectedPanel.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DnetConnectedPanel.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDnetConnectedPanel(this IServiceCollection services)
        {
            services.AddScoped(typeof(IConnectedPanelService), typeof(ConnectedPanelService));

            services.AddTransient<IViewportRuler, ViewportRuler>();

            return services;
        }
    }
}
