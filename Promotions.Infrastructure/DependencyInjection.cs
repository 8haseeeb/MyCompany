using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Application.Products.Interfaces;
using Promotions.Application.ProductDetails.Interfaces;
using Promotions.Application.PromoArticles.Interfaces;
using Promotions.Application.PromoActions.Interfaces;
using Promotions.Application.Interfaces;
using Promotions.Infrastructure.Persistence;
using Promotions.Infrastructure.Persistence.Repositories;

namespace Promotions.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            
            services.AddDbContext<PromotionsDbContext>(options =>
                options.UseSqlServer(connectionString) 
              );

          
            services.AddScoped<IPromoMeasureFieldRepository, PromoMeasureFieldRepository>();
            services.AddScoped<IDeliveryPointRepository, DeliveryPointRepository>();
            services.AddScoped<ICustomerRelationRepository, CustomerRelationRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPromoActionRepository, PromoActionRepository>();
            services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
            services.AddScoped<IPromoArticleRepository, PromoArticleRepository>();

            return services;
        }
    }
}
