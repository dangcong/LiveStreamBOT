using BaseSouce.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Data.Entities
{
    public class StreamHistoryLog
    {
        public int Id { get; set; }
        public int StreamHistoryId { get; set; }
        public DateTime CreatedTime { get; set; }
        public SteamLogStatus LogStatus { get; set; }
        public string Message { get; set; }

        public StreamHistory StreamHistory { get; set; }
    }
}
