using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class PromoDeliveryPointConfiguration
        : IEntityTypeConfiguration<PromoDeliveryPoint>
    {
        public void Configure(EntityTypeBuilder<PromoDeliveryPoint> builder)
        {
            // ================================
            // TABLE
            // ================================
            builder.ToTable("TA5014DELIVERYPOINTS");

            // ================================
            // COMPOSITE PRIMARY KEY
            // ================================
            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodDeliveryPoint
            });

            // ================================
            // COLUMNS
            // ================================

            builder.Property(x => x.IdAction)
                .HasColumnName("ID_ACTION")
                .IsRequired();

            builder.Property(x => x.CodDeliveryPoint)
                .HasColumnName("CODDELIVERYPOINT")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.FlgInclusion)
                .HasColumnName("FLGINCLUSION")
                .IsRequired();

            // ================================
            // INDEX (Optional but recommended)
            // ================================
            builder.HasIndex(x => x.IdAction)
                .HasDatabaseName("IX_TA5014DELIVERYPOINTS_ID_ACTION");
        }
    }
}
