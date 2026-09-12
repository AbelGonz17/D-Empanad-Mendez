using DMendez.Domain.Interfaces;
using DMendez.Infrastructure.Contex;
using DMendez.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DMendez.Infrastructure
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DMendezDbContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure());
            });

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IComboRepository, ComboRepository>();
            services.AddScoped<IDeliveryZoneRepository, DeliveryZoneRepository>();
            services.AddScoped<IProductInventoryRepository, ProductInventoryRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // External Services
            services.AddScoped<DMendez.Application.Interfaces.External.IFileStorageService, DMendez.Infrastructure.Services.LocalFileStorageService>();

            // External Services: Email
            services.AddHttpClient("Resend");

            var emailSection = configuration.GetSection("EmailSettings");
            var useMock = emailSection.GetValue<bool>("UseMock", true);
            var provider = emailSection.GetValue<string>("Provider") ?? "Resend";

            if (useMock)
            {
                services.AddScoped<DMendez.Application.Interfaces.External.IEmailService, DMendez.Infrastructure.Services.Email.MockEmailService>();
            }
            else if (provider.Equals("Smtp", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<DMendez.Application.Interfaces.External.IEmailService, DMendez.Infrastructure.Services.Email.SmtpEmailService>();
            }
            else
            {
                services.AddScoped<DMendez.Application.Interfaces.External.IEmailService, DMendez.Infrastructure.Services.Email.ResendEmailService>();
            }

            return services;
        }
    }
}