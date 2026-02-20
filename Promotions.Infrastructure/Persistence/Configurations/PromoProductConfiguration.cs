using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Products;

namespace Promotions.Infrastructure.Products.Configurations
{
    public class PromoProductConfiguration : IEntityTypeConfiguration<PromoProduct>
    {
        public void Configure(EntityTypeBuilder<PromoProduct> builder)
        {
            builder.ToTable("TA502PRODUCTS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodProduct,
                x.LevProduct,
                x.CodDisplay
            });

            builder.Property(x => x.IdAction).HasColumnName("IDACTION").HasColumnType("int");
            builder.Property(x => x.CodProduct).HasColumnName("CODPRODUCT").HasMaxLength(50);
            builder.Property(x => x.LevProduct).HasColumnName("LEVPRODUCT").HasColumnType("int");
            builder.Property(x => x.CodDisplay).HasColumnName("CODDISPLAY").HasMaxLength(50);
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").IsRequired().HasMaxLength(50);
            builder.Property(x => x.QtyEstimated).HasColumnName("QTYESTIMATED");
            builder.Property(x => x.PerceDiscount1).HasColumnName("PERCDISCOUNT1");
            builder.Property(x => x.PerceDiscount2).HasColumnName("PERCDISCOUNT2");
            builder.Property(x => x.NumMeasure).HasColumnName("NUMMEASUREA");
            builder.Property(x => x.CodMeasure).HasColumnName("CODMEASURE").HasMaxLength(20);

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
