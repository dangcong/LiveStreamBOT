using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSouce.Data.Configurations
{
    public class ManagerBOTConfiguration : IEntityTypeConfiguration<ManagerBOT>
    {
        public void Configure(EntityTypeBuilder<ManagerBOT> builder)
        {
            builder.ToTable("ManagerBOTs");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Manager).WithMany(x => x.ManagerBOTs).HasForeignKey(x => x.UserManager);
        }
    }
}
