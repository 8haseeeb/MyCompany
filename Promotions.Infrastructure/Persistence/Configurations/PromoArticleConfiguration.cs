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

          
            builder.HasKey(x => new
            {
                x.CodDiv,
                x.CodNode
            });

          
            builder.Property(x => x.CodDiv)
                .HasColumnName("CODDIV")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.CodNode)
                .HasColumnName("CODNODE")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CodNode1)
                .HasColumnName("CODNODE1")
                .HasMaxLength(50);

            builder.Property(x => x.CodNode2)
                .HasColumnName("CODNODE2")
                .HasMaxLength(50);

            builder.Property(x => x.CodNodeN)
                .HasColumnName("CODNODE_N")
                .HasMaxLength(50);
        }
    }
}
