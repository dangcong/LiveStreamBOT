using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData.Classes
{
    public class DiskInfo
    {
        public long Used { get; set; }
        public long Total { get; set; }
        public double GetPercent()
        {
            if (Used == 0 || Total == 0) return 0;
            return Used * 1.0 / Total;
        }
    }
}
