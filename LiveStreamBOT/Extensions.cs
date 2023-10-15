using BaseSouce.SharedSignalrData.Classes;
using LiveStreamBOT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT
{
    internal static class Extensions
    {
        internal static ClientInfo GetClientInfo(this ISystemInfoReader systemInfoReader)
            => new ClientInfo()
            {
                CpuUsage = systemInfoReader.GetPercentCpu(),
                RamUsage = systemInfoReader.GetPercentRam(),
                Bandwidth = systemInfoReader.GetBandwidth(),
                DiskInfo = systemInfoReader.GetDiskInfo(),
            };
    }
}
