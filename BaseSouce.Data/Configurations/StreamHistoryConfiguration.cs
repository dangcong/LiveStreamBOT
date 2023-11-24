using BaseSource.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseSource.Data.Configurations
{
    public class StreamHistoryConfiguration : IEntityTypeConfiguration<StreamHistory>
    {
        public void Configure(EntityTypeBuilder<StreamHistory> builder)
        {
            builder.ToTable("StreamHistorys");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BotId).HasMaxLength(200);
            builder.Property(x => x.StreamKey).HasMaxLength(200);
            builder.Property(x => x.RTMP).HasMaxLength(200);
            builder.Property(x => x.Video).HasMaxLength(200);
            builder.Property(x => x.Audio).HasMaxLength(200);
            builder.Property(x => x.ErrorMessage).HasMaxLength(200);

            builder.HasOne(x => x.AppUser).WithMany(x => x.StreamHistorys).HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.ManagerBOTBackup).WithMany(x => x.StreamHistoryBackups).HasForeignKey(x => x.ManagerBOTBackupId);
        }
    }
}
