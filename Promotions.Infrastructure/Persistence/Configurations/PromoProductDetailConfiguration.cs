using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.ProductDetails;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class PromoProductDetailConfiguration
        : IEntityTypeConfiguration<PromoProductDetail>
    {
        public void Configure(EntityTypeBuilder<PromoProductDetail> builder)
        {
            builder.ToTable("TA5026PRODUCTDETAILS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodProduct,
                x.LevProduct,
                x.CodDisplay,
                x.CodNode,
                x.CodDiv
            });

            builder.Property(x => x.IdAction).HasColumnName("IDACTION").HasColumnType("int");
            builder.Property(x => x.CodProduct).HasColumnName("CODPRODUCT").HasMaxLength(50);
            builder.Property(x => x.LevProduct).HasColumnName("LEVPRODUCT").HasColumnType("int");
            builder.Property(x => x.CodDisplay).HasColumnName("CODDISPLAY").HasMaxLength(50);
            builder.Property(x => x.CodNode).HasColumnName("CODNODEO").HasMaxLength(30);
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(50);
            builder.Property(x => x.FlgInclusion).HasColumnName("FLGINCLUSION");


            // The FK to TA5150PROMOARTICLES is intentionally not enforced at the EF level.
            // PromoArticles are master/catalog data managed externally and are not
            // created as part of the atomic promotion creation flow.

            builder.HasOne(x => x.Product)
                   .WithMany(p => p.Details)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay })
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
