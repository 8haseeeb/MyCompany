using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Measures;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class PromoMeasureFieldConfiguration
        : IEntityTypeConfiguration<PromoMeasureField>
    {
        public void Configure(EntityTypeBuilder<PromoMeasureField> builder)
        {
            builder.ToTable("TA5118PROMOMEASUREFIELDS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodProduct,
                x.LevProduct,
                x.CodDisplay,
                x.CodDiv,
                x.CodMeasure,
                x.FieldName
            });

            // Column configurations
            builder.Property(x => x.CodProduct).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CodDisplay).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(20).IsRequired();
            builder.Property(x => x.CodMeasure).HasColumnName("CODMEASURE").HasMaxLength(50).IsRequired();
            builder.Property(x => x.FieldName).HasColumnName("FIELDNAME").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Formula).HasColumnName("FORMULA").HasMaxLength(500);

            // Relationship to PromoProduct
            builder.HasOne(x => x.Product)
                   .WithMany(x => x.MeasureFields)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay });
        }
    }
}
