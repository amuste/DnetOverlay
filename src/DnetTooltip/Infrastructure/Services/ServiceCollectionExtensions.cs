using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services;
using DnetTooltip.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DnetTooltip.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTooltip(this IServiceCollection services)
        {
            services.AddScoped(typeof(ITooltipService), typeof(TooltipService));

            services.AddTransient<IViewportRuler, ViewportRuler>();

            return services;
        }
    }
}
