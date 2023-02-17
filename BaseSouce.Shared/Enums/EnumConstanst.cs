using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Shared.Enums
{
    public enum ServicePackType : byte
    {
        _1080 = 1,
        _4K = 2,
    }
    public enum ManagerBOTStatus : byte
    {
        Connected = 1,
        Disconnected = 2,
    }
    public enum OrderStatus : byte
    {
        Success = 1,
        Waiting = 2,
        Cancel = 3
    }
}
