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
    }
}
