using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSouce.Data.Configurations
{
    public class ServicePackByMonthConfiguration : IEntityTypeConfiguration<ServicePackByMonth>
    {

        public void Configure(EntityTypeBuilder<ServicePackByMonth> builder)
        {
            builder.ToTable("ServicePackByMonth");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ServicePack).WithMany(x => x.ServicePackByMonths).HasForeignKey(x => x.ServicePackId);
        }
    }
}
