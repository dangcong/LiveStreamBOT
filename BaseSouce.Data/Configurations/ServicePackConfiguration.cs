using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSouce.Data.Configurations
{
    public class ServicePackConfiguration : IEntityTypeConfiguration<ServicePack>
    {
        public void Configure(EntityTypeBuilder<ServicePack> builder)
        {
            builder.ToTable("ServicePacks");

            builder.HasKey(x => x.Id);

        }
    }
}
