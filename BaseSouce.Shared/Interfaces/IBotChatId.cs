using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Interfaces
{
    public interface IBotChatId
    {
        IEnumerable<string> TelegramUserNames { get; }
    }
}
