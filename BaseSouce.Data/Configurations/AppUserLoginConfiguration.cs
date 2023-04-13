using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BaseSouce.Data.Configurations
{
    public class AppUserLoginConfiguration : IEntityTypeConfiguration<AppUserLogin>
    {
        public void Configure(EntityTypeBuilder<AppUserLogin> builder)
        {
            builder.ToTable("AppUserLogins");
        }
    }
}
