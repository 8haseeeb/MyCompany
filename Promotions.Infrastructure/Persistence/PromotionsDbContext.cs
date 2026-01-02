using Microsoft.EntityFrameworkCore;
using Promotions.Domain.Articles;
using Promotions.Domain.CustomerRelations;
using Promotions.Domain.DeliveryPoints;
using Promotions.Domain.Measures;
using Promotions.Domain.Participants;
using Promotions.Domain.ProductDetails;
using Promotions.Domain.Products;
using Promotions.Domain.PromoActions;
using Promotions.Domain.Articles;
using Promotions.Infrastructure.Persistence.Configurations;
using Promotions.Infrastructure.Products.Configurations;


namespace Promotions.Infrastructure.Persistence
{
    public class PromotionsDbContext : DbContext
    {
        public PromotionsDbContext(DbContextOptions<PromotionsDbContext> options)
            : base(options) { }

        public DbSet<PromoMeasureField> PromoMeasureFields { get; set; } = null!;

        public DbSet<PromoParticipants> Participants { get; set; } = null!;

        public DbSet<PromoDeliveryPoint> DeliveryPoints { get; set; } = null!;
        public DbSet<CustomerRelation> CustomerRelations { get; set; } = null!;

        public DbSet<PromoProduct> Products { get; set; } = null!;

        public DbSet<PromoAction> PromoActions { get; set; } = null!;

        public DbSet<PromoProductDetail> ProductDetails { get; set; } = null!;

        public DbSet<PromoArticle> PromoArticles { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PromoMeasureFieldConfiguration());
            modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
            modelBuilder.ApplyConfiguration(new PromoDeliveryPointConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerRelationConfiguration());
            modelBuilder.ApplyConfiguration(new PromoProductConfiguration());
            modelBuilder.ApplyConfiguration(new PromoActionConfiguration());
            modelBuilder.ApplyConfiguration(new PromoProductDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PromoArticleConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
