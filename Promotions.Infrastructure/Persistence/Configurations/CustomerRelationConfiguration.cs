using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Infrastructure.Persistence.Configurations
{
    public class CustomerRelationConfiguration
        : IEntityTypeConfiguration<CustomerRelation>
    {
        public void Configure(EntityTypeBuilder<CustomerRelation> builder)
        {
            builder.ToTable("TB0042RELATIONS_CUST");

            builder.HasKey(x => new
            {
                x.CodHier,
                x.CodDiv,
                x.CodNode,
                x.IdLevel,
                x.DteStart
            });

            builder.Property(x => x.CodHier).HasColumnName("CODHER").HasMaxLength(10);
            builder.Property(x => x.CodDiv).HasColumnName("CODDIV").HasMaxLength(10);
            builder.Property(x => x.CodNode).HasColumnName("CODNODE").HasMaxLength(30);
            builder.Property(x => x.IdLevel).HasColumnName("IDLEVEL");
            builder.Property(x => x.DteStart).HasColumnName("DTESTART");
            
            builder.Property(x => x.CodParentNode).HasColumnName("COOPAREANTNODE").HasMaxLength(30);
            builder.Property(x => x.DteEnd).HasColumnName("DTEEND");
        }
    }
}
