using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Participants;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<PromoParticipants>
    {
        public void Configure(EntityTypeBuilder<PromoParticipants> builder)
        {
            builder.ToTable("TA5012PARTICIPANTS");

            builder.HasKey(p => new { p.IdAction, p.CodParticipant });

            builder.Property(p => p.IdAction).HasColumnName("ID_ACTION");
            builder.Property(p => p.CodParticipant).HasColumnName("CODPARTICIPANT");
            builder.Property(p => p.FlgInclusion).HasColumnName("FLGINCLUSION");

            // Foreign Key Properties for CustomerRelation
            builder.Property(p => p.CodHier).HasMaxLength(10).IsRequired();
            builder.Property(p => p.CodDiv).HasMaxLength(10).IsRequired();
            builder.Property(p => p.CodNode).HasMaxLength(30).IsRequired();

            // Relationships
            builder.HasOne(p => p.Relation)
                   .WithMany(r => r.Participants)
                   .HasForeignKey(p => new { p.CodHier, p.CodDiv, p.CodNode, p.IdLevel, p.DteStart });
        }
    }
}
