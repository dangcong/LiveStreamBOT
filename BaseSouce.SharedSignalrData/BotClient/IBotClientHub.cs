using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData.BotClient
{
    public interface IBotClientHub
    {
        Task RenderAsync(RenderCommand renderCommand);
    }
}
