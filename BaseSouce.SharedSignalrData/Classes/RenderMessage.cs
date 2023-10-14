using BaseSouce.SharedSignalrData.Enums;

namespace BaseSouce.SharedSignalrData.Classes
{
    public class RenderMessage : Identity
    {
        /// <summary>
        /// for <see cref="RenderStatus.LiveStreaming"/>
        /// </summary>
        public RenderInfo RenderInfo { get; set; }


        /// <summary>
        /// 0-100<br>
        /// </br>for <see cref="RenderStatus.Downloading"/>
        /// </summary>
        public double DownloadPercentage { get; set; }


        /// <summary>
        /// for <see cref="RenderStatus.Error"/>
        /// </summary>
        public ExceptionInfo ExceptionInfo { get; set; }


        public RenderStatus RenderStatus { get; set; }
    }
}
