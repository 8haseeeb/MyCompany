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

            builder.Property(x => x.CodProduct).HasMaxLength(50);
            builder.Property(x => x.CodDisplay).HasMaxLength(50);
            builder.Property(x => x.CodNode).HasMaxLength(50);
            builder.Property(x => x.CodDiv).HasMaxLength(50);
        }
    }
}
