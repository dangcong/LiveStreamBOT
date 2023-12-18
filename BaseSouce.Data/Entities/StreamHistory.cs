using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Enums;

namespace BaseSource.Data.Entities
{
    public class StreamHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? ManagerBOTId { get; set; }
        public string BotId { get; set; }
        public int? ManagerBOTBackupId { get; set; }
        public string BotBackupId { get; set; }
        public string StreamKey { get; set; }
        public string RTMP { get; set; }
        public string StreamName { get; set; }
        public LiveType LiveType { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public LiveOrder LiveOrder { get; set; }
        public bool IsForever { get; set; }
        public bool IsLiveNow { get; set; }
        public DateTime? StartTimeLive { get; set; }
        public DateTime? EndTimeLive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? Updatedtime { get; set; }
        public DateTime? DisconnectedTime { get; set; }
        public bool IsLiveBackup { get; set; }
        public int? StreamBackupId { get; set; }
        public StreamStatus Status { get; set; }
        public bool IsSendNotification { get; set; }
        public int TimeBackup { get; set; }
        public PlatformType PlatformType { get; set; }
        public RenderStatus RenderStatus { get; set; }
        public string ErrorMessage { get; set; }

        public ManagerBOT ManagerBOT { get; set; }
        public AppUser AppUser { get; set; }
        public ManagerBOT ManagerBOTBackup { get; set; }
    }
}
