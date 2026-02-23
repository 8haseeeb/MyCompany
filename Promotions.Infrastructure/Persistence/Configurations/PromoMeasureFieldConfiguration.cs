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
                x.CodDiv,
                x.CodMeasure,
                x.FieldName
            });



            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(10).IsRequired();
            builder.Property(x => x.CodMeasure).HasColumnName("CODMEASURE").HasMaxLength(50).IsRequired();
            builder.Property(x => x.FieldName).HasColumnName("FIELDNAME").HasMaxLength(100).IsRequired();
            builder.Property(x => x.Formula).HasColumnName("FORMULA").HasMaxLength(500);
        }
    }
}
