using DnetDialogComponent.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DnetDialogComponent.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDialog(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDialogService), typeof(DialogService));

            return services;
        }
    }
}
