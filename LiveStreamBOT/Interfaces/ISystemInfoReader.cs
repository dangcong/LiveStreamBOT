using BaseSouce.SharedSignalrData.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT.Interfaces
{
    internal interface ISystemInfoReader : IDisposable
    {
        double GetPercentCpu();
        double GetPercentRam();
        double GetBandwidth();
        DiskInfo GetDiskInfo();
    }
}
