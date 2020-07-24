using DnetDialogComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DnetDialogComponent.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDialog(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDialogService), typeof(DialogService));

            services.AddTransient<IViewportRuler, ViewportRuler>();

            return services;
        }
    }
}
