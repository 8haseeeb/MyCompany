using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Participants;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<PromoParticipants>
    {
        public void Configure(EntityTypeBuilder<PromoParticipants> builder)
        {
            builder.ToTable("TA8012PARTICIPANTS");

            builder.HasKey(p => new { p.IdAction, p.CodParticipant });

            builder.Property(p => p.IdAction).HasColumnName("IDACTION");
            builder.Property(p => p.CodParticipant).HasColumnName("CODPARTICIPANT");
            builder.Property(p => p.FlgInclusion).HasColumnName("FLGINCLUSION");

            // Mapping hierarchy fields explicitly to match screenshot exactly, 
            // even if snapshot defaults to property name.
            builder.Property(p => p.CodHier).HasColumnName("CODHER").HasMaxLength(10);
            builder.Property(p => p.CodDiv).HasColumnName("CODDIV").HasMaxLength(10);
            builder.Property(p => p.CodNode).HasColumnName("CODNODE").HasMaxLength(30);
            builder.Property(p => p.IdLevel).HasColumnName("IDLEVEL").IsRequired();
            builder.Property(p => p.DteStart).HasColumnName("DTESTART").IsRequired();

            builder.HasOne(p => p.Action)
                   .WithMany(a => a.Participants)
                   .HasForeignKey(p => p.IdAction)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Relation)
                   .WithMany(r => r.Participants)
                   .HasForeignKey(p => new { p.CodHier, p.CodDiv, p.CodNode, p.IdLevel, p.DteStart })
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
