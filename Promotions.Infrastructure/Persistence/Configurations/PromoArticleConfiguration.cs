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
            builder.ToTable("TA5150PROMOARTICLE");

            builder.HasKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodDiv, x.CodNode });

            builder.Property(x => x.IdAction).HasColumnType("int");
            builder.Property(x => x.CodProduct).HasMaxLength(50);
            builder.Property(x => x.LevProduct).HasColumnType("int");
            builder.Property(x => x.CodDisplay).HasMaxLength(50);
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(50);
            builder.Property(x => x.CodNode).HasColumnName("CODNODE").HasMaxLength(50);
            builder.Property(x => x.CodNode1).HasColumnName("CODNODE1").HasMaxLength(50);
            builder.Property(x => x.CodNode2).HasColumnName("CODNODE2").HasMaxLength(50);
            builder.Property(x => x.CodNodeN).HasColumnName("CODNODE_N").HasMaxLength(50);

            builder.HasOne(x => x.ProductDetail)
                   .WithMany(x => x.Articles)
                   .HasForeignKey(x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodNode, x.CodDiv })
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
