using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services;
using DnetToastComponent.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DnetToastComponent.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDnetToast(this IServiceCollection services)
        {
            services.AddScoped(typeof(IToastService), typeof(ToastService));

            services.AddTransient<IViewportRuler, ViewportRuler>();

            return services;
        }
    }
}
