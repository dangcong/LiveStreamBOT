using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class ManagerGroupConfiguration : IEntityTypeConfiguration<ManagerGroup>
    {
        public void Configure(EntityTypeBuilder<ManagerGroup> builder)
        {
            builder.ToTable("ManagerGroups");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Note).HasMaxLength(100);
            builder.HasOne(x => x.UserManager).WithMany(x => x.ManagerGroups).HasForeignKey(x => x.UserIdManager);
            builder.HasOne(x => x.User).WithMany(x => x.UserGroups).HasForeignKey(x => x.UserId);
        }
    }
}
