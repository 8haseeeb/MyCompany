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

            builder.Property(x => x.IdAction).HasColumnType("int");
            builder.Property(x => x.CodProduct).HasMaxLength(50);
            builder.Property(x => x.LevProduct).HasColumnType("int");
            builder.Property(x => x.CodDisplay).HasMaxLength(50);
            builder.Property(x => x.CodDiv).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CodMeasure).HasMaxLength(20);

            builder.HasOne(x => x.Action)
                   .WithMany(a => a.Products)
                   .HasForeignKey(x => x.IdAction)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Details)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay });
        }
    }
}
