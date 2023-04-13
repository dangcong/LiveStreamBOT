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
    public enum LiveType : byte
    {
        _1080 = 1,
        _4K = 2,
    }
    public enum LiveOrder : byte
    {
        Sequence = 1,
        Random = 2
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
    public enum StreamStatus : byte
    {
        Streaming = 1,
        Stop = 2,
    }
    public enum SteamLogStatus : byte
    {
        Download = 1,
        Render = 2,
        Running = 3
    }
}
