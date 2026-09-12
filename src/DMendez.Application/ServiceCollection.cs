using DMendez.Application.Interfaces;
using DMendez.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DMendez.Application
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IComboService, ComboService>();
            services.AddScoped<IDeliveryZoneService, DeliveryZoneService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
