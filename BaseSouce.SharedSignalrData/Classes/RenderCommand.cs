using BaseSouce.SharedSignalrData.Enums;

namespace BaseSouce.SharedSignalrData.Classes
{
    public class RenderCommand : Identity
    {
        public string RenderArguments { get; set; }
        public string LiveStreamArguments { get; set; }
        public List<FileData> FileDatas { get; set; }
        public Command Command { get; set; }
        public FileType FileType { get; set; }
        public DateTime? UtcStartTime { get; set; }
        public DateTime? UtcEndTime { get; set; }
    }
}
