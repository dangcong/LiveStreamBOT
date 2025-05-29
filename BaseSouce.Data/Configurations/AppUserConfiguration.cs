using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSouce.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");
            builder.Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            builder.Property(x => x.LinkFB).HasMaxLength(500);
            builder.Property(x => x.TelegramUserId).HasMaxLength(500);
            builder.Property(x => x.DiscordUserId).HasMaxLength(500);
            builder.Property(x => x.TelegramUserIdReceiveNotificationsFromStaff).HasMaxLength(500);
            builder.Property(x => x.DiscordUserIdReceiveNotificationsFromStaff).HasMaxLength(500);

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.AppUserLogins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

          
        }
    }
}
