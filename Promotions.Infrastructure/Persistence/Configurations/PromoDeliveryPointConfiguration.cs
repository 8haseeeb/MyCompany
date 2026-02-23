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
            builder.ToTable("TA501DELIVERYPOINTS");

            builder.HasKey(x => new
            {
                x.IdAction,
                x.CodDeliveryPoint
            });

            builder.Property(x => x.IdAction)
                .HasColumnName("IDACTION")
                .IsRequired();

            builder.Property(x => x.CodDeliveryPoint)
                .HasColumnName("CODDELIVERYPOINT");

            builder.Property(x => x.FlgInclusion)
                .HasColumnName("FLGINCLUSION")
                .IsRequired();
            
            builder.Property(d => d.CodHier).HasColumnName("CODHER").HasMaxLength(10);
            builder.Property(d => d.CodDiv).HasColumnName("CODDIV").HasMaxLength(50).IsRequired();
            builder.Property(x => x.CodNode).HasColumnName("CODNODE").HasMaxLength(30).IsRequired();
            builder.Property(x => x.IdLevel).HasColumnName("IDLEVEL").IsRequired();
            builder.Property(x => x.DteStart).HasColumnName("DTESTART").IsRequired();

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
