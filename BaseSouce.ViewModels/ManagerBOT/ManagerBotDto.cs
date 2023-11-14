using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
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
        public LiveType? LiveType { get; set; }
        public string UserManager { get; set; }
    }

    public class ManagerBotInfoDto
    {
        public int Id { get; set; }
        public string BotId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Manager { get; set; }
        public string UserManager { get; set; }
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
        public LiveType? LiveType { get; set; }
        public int TotalUser { get; set; }
        public long DiskUsed { get; set; }
        public long DiskTotal { get; set; }
    }
    public class ManagerBOTRequestDto : PageQuery
    {
        public string Manager { get; set; }
    }

    public class UserBotDto
    {
        public string BotName { get; set; }
        public string UserName { get; set; }
        public LiveType? Type { get; set; }
        public int BotId { get; set; }
        public int NumberOfThreads { get; set; }
        public int NumberOfThreadsInRun { get; set; }
        public string UserId { get; set; }
        public string Group { get; set; }
    }
}
