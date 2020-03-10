﻿using DnetOverlayComponent.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DnetOverlayComponent.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDnetOverlay(this IServiceCollection services)
        {
            services.AddScoped(typeof(IOverlayService), typeof(OverlayService));

            return services;
        }
    }
}