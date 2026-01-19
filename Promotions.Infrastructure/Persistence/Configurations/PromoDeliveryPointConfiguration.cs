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
            builder.ToTable("TA5014DELIVERYPOINTS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodDeliveryPoint
            });

            builder.Property(x => x.IdAction)
                .HasColumnName("ID_ACTION")
                .IsRequired();

            builder.Property(x => x.CodDeliveryPoint)
                .HasColumnName("CODDELIVERYPOINT")
                .HasMaxLength(50);

            builder.Property(x => x.FlgInclusion)
                .HasColumnName("FLGINCLUSION")
                .IsRequired();

            builder.Property(x => x.CodHier).HasMaxLength(10).IsRequired();
            builder.Property(x => x.CodDiv).HasMaxLength(10).IsRequired();
            builder.Property(x => x.CodNode).HasMaxLength(30).IsRequired();
            builder.Property(x => x.IdLevel).IsRequired();
            builder.Property(x => x.DteStart).IsRequired();

            builder.HasOne(x => x.Action)
                   .WithMany(a => a.DeliveryPoints)
                   .HasForeignKey(x => x.IdAction)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Relation)
                   .WithMany(r => r.DeliveryPoints)
                   .HasForeignKey(x => new { x.CodHier, x.CodDiv, x.CodNode, x.IdLevel, x.DteStart })
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
