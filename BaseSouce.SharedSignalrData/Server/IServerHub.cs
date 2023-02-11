using BaseSouce.SharedSignalrData.BotClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData.Server
{
    public interface IServerHub
    {
        Task RenderUpdateAsync(RenderMessage renderMessage);
    }
}
