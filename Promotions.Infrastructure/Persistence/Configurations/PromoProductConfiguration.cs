using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Products;

namespace Promotions.Infrastructure.Products.Configurations
{
    public class PromoProductConfiguration : IEntityTypeConfiguration<PromoProduct>
    {
        public void Configure(EntityTypeBuilder<PromoProduct> builder)
        {
            builder.ToTable("TA5020PRODUCTS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodProduct,
                x.LevProduct,
                x.CodDisplay
            });

            builder.Property(x => x.CodProduct).HasMaxLength(50);
            builder.Property(x => x.CodDisplay).HasMaxLength(50);
            builder.Property(x => x.CodDiv).HasMaxLength(50);
            builder.Property(x => x.CodMeasure).HasMaxLength(20);

        // Relationships
        builder.HasMany(x => x.Details)
               .WithOne(x => x.Product)
               .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay });
        }
    }
}
