using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT.DataClass
{
    internal class SettingData
    {
        public string BotId { get; set; } = Guid.NewGuid().ToString();
        public string NetworkName { get; set; } = "eth0";
        public int PingInterval { get; set; } = 1000;
    }
}
