using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
            // Legacy DB allows NULL; mapping NULL → false avoids SqlNullValueException on GET.
            var inclusionConverter = new ValueConverter<bool, bool?>(
                v => v,
                v => v ?? false);
            builder.Property(x => x.FlgInclusion)
                .HasColumnName("FLGINCLUSION")
                .HasConversion(inclusionConverter);

            // Optional match to TA5150PROMOARTICLES — DB FK was dropped; details may reference missing catalog rows.
            builder.HasOne(x => x.Article)
                .WithMany()
                .HasForeignKey(x => new { x.CodDiv, x.CodNode })
                .HasPrincipalKey(x => new { x.CodDiv, x.CodNode })
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product)
                   .WithMany(p => p.Details)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay })
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
