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

            builder.HasKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodDiv, x.CodNode });

            builder.Property(x => x.IdAction).HasColumnName("IDACTION").HasColumnType("int");
            builder.Property(x => x.CodProduct).HasColumnName("CODPRODUCT").HasMaxLength(50);
            builder.Property(x => x.LevProduct).HasColumnName("LEVPRODUCT").HasColumnType("int");
            builder.Property(x => x.CodDisplay).HasColumnName("CODDISPLAY").HasMaxLength(50);
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(50);
            builder.Property(x => x.CodNode).HasColumnName("CODNODEO").HasMaxLength(50);
            builder.Property(x => x.CodNode1).HasColumnName("CODNODE1").HasMaxLength(50);
            builder.Property(x => x.CodNode2).HasColumnName("CODNODE2").HasMaxLength(50);
            builder.Property(x => x.CodNodeN).HasColumnName("FROMNODEFIN").HasMaxLength(50);


            builder.HasOne(x => x.ProductDetail)
                   .WithMany(x => x.Articles)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodNode, x.CodDiv })
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
