using DnetOverlayComponent.Infrastructure.Interfaces;
using DnetOverlayComponent.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DnetAutocompleteComponent.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutocompleteList(this IServiceCollection services)
        {
            services.AddTransient<IViewportRuler, ViewportRuler>();

            return services;
        }
    }
}
