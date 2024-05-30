using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData.Classes
{
    public class SettingInfo
    {
        public TimeSpan ScheduleTimeOnDownloadFailed { get; set; } = TimeSpan.FromMinutes(10);
        public int PingInterval { get; set; } = 5000;
        public string? NetworkName { get; set; } = null;
    }
}
