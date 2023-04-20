using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Server;
using BaseSouce.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.ManagerBOT
{
    public class ManagerBotCreateDto : ClientInfo
    {
        public string ConnectionId { get; set; }
        public string BotId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }

    public class ManagerBotUpdateDto : ManagerBotCreateDto
    {
        public int Id { get; set; }
        public ManagerBOTStatus Status { get; set; }
        public int NumberOfThreads { get; set; }
        public int Retry { get; set; }
    }

    public class ManagerBotInfoDto
    {
        public int Id { get; set; }
        public string BotId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public ManagerBOTStatus Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int Retry { get; set; }

        public int NumberOfThreads { get; set; }
        public int NumberOfThreadsInRun { get; set; }
        public double CPU { get; set; }
        public double RAM { get; set; }
        public double Bandwidth { get; set; }
        public bool IsUsed { get; set; }
    }
    public class ManagerBOTRequestDto : PageQuery
    {

    }
}
