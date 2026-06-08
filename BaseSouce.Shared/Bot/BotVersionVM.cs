using System;

namespace BaseSource.Shared.Bot
{
    /// <summary>
    /// Thông tin phiên bản cập nhật của bot (dùng chung giữa server API và bot client).
    /// </summary>
    public class BotVersionVM
    {
        public string Version { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Đường dẫn tương đối tới file zip (vd: updates/{botName}/{rid}/{version}.zip).
        /// Bot ghép với BaseAddress (ApiDomain) để tải.
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;

        /// <summary>
        /// Runtime Identifier của bản build (win-x64, linux-x64...).
        /// </summary>
        public string Rid { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; }
    }
}
