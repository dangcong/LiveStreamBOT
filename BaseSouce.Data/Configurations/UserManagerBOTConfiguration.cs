
using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class UserManagerBOTConfiguration : IEntityTypeConfiguration<UserManagerBOT>
    {
        public void Configure(EntityTypeBuilder<UserManagerBOT> builder)
        {
            builder.ToTable("UserManagerBOTs");

            builder.HasKey(x => x.Id);

           

            builder.HasOne(x => x.User).WithMany(x => x.UserManagerBOTs).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.ManagerBOT).WithMany(x => x.UserManagerBOTs).HasForeignKey(x => x.ManagerBOTId);
        }
    }
}
