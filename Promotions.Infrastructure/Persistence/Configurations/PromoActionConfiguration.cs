using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.PromoActions;

public class PromoActionConfiguration : IEntityTypeConfiguration<PromoAction>
{
    public void Configure(EntityTypeBuilder<PromoAction> builder)
    {
        builder.ToTable("TA500PROMOACTION");

        builder.HasKey(x => x.IdAction);

        builder.Property(x => x.IdAction)
            .HasColumnName("ID_ACTION")
            .ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("DESACTION");
        builder.Property(x => x.CodDiv).HasColumnName("CODDIV");
        builder.Property(x => x.CodContractor).HasColumnName("CODCONTRACTOR");

        builder.Property(x => x.DteStartSellIn).HasColumnName("DTESTARTSELLIN");
        builder.Property(x => x.DteEndSellIn).HasColumnName("DTEENDSELLIN");
        builder.Property(x => x.DteStartSellOut).HasColumnName("DTESTARTSELLOUT");
        builder.Property(x => x.DteEndSellOut).HasColumnName("DTEENDSELLOUT");

        builder.Property(x => x.DocumentKey).HasColumnName("DOCUMENTKEY");
        builder.Property(x => x.DteToShost).HasColumnName("DTETOSHOST");
        builder.Property(x => x.LevParticipants).HasColumnName("LEVPARTICIPANTS");

        // Relationships
        builder.Property(x => x.ContractorCodHier).HasColumnName("CONTRACTORCODHIER");
        builder.Property(x => x.ContractorIdLevel).HasColumnName("CONTRACTORIDLEVEL");
        builder.Property(x => x.ContractorDteStart).HasColumnName("CONTRACTORDTESTART");

        // Relationships
        builder.HasOne(x => x.Contractor)
               .WithMany()
               .HasForeignKey(x => new { x.ContractorCodHier, x.CodDiv, x.CodContractor, x.ContractorIdLevel, x.ContractorDteStart });

        builder.HasMany(x => x.Products)
               .WithOne(x => x.Action)
               .HasForeignKey(x => x.IdAction);

        builder.HasMany(x => x.Participants)
               .WithOne(x => x.Action)
               .HasForeignKey(x => x.IdAction);

        builder.HasMany(x => x.DeliveryPoints)
               .WithOne(x => x.Action)
               .HasForeignKey(x => x.IdAction);
    }
}
