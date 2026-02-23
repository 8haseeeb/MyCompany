using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.Articles;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class PromoArticleConfiguration
        : IEntityTypeConfiguration<PromoArticle>
    {
        public void Configure(EntityTypeBuilder<PromoArticle> builder)
        {
            builder.ToTable("TA5150PROMOARTICLES");

            builder.HasKey(x => new { x.CodDiv, x.CodNode });
            
            // Ignore phantom properties not in the legacy table
            builder.Ignore(x => x.IdAction);
            builder.Ignore(x => x.CodProduct);
            builder.Ignore(x => x.LevProduct);
            builder.Ignore(x => x.CodDisplay);

            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(50);
            builder.Property(x => x.CodNode).HasColumnName("CODNODEO").HasMaxLength(50);
            builder.Property(x => x.CodNode1).HasColumnName("CODNODE1").HasMaxLength(50);
            builder.Property(x => x.CodNode2).HasColumnName("CODNODE2").HasMaxLength(50);
            builder.Property(x => x.CodNodeN).HasColumnName("FROMNODEFIN").HasConversion<bool>(); // BIT column
        }
    }
}
