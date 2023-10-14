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

namespace BaseSource.ViewModels.Stream
{
    public class StreamCreateDto
    {
        public string UserId { get; set; }
        public int ManagerBOTId { get; set; }
        public string BotId { get; set; }
        public string StreamKey { get; set; }
        public string RTMP { get; set; }
        public string StreamName { get; set; }
        public LiveType LiveType { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public LiveOrder LiveOrder { get; set; }
        public bool IsForever { get; set; }
        public bool IsLiveNow { get; set; }
        public DateTime? StartTimeLive { get; set; }
        public DateTime? EndTimeLive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? Updatedtime { get; set; }
        public StreamStatus Status { get; set; }
        public PlatformType PlatformType { get; set; }
    }


    public class StreamClientInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string BotName { get; set; }
        public int ManagerBOTId { get; set; }
        public string BotId { get; set; }
        public string StreamKey { get; set; }
        public string RTMP { get; set; }
        public string StreamName { get; set; }
        public LiveType LiveType { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public LiveOrder LiveOrder { get; set; }
        public bool IsForever { get; set; }
        public bool IsLiveNow { get; set; }
        public DateTime? StartTimeLive { get; set; }
        public DateTime? EndTimeLive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? Updatedtime { get; set; }
        public StreamStatus Status { get; set; }
        public PlatformType PlatformType { get; set; }
        public RenderStatus RenderStatus { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class StreamClientRequestDto : PageQuery
    {

    }
    public class StreamUpdateDto : StreamCreateDto
    {

    }

    public class StreamAdminInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int ManagerBOTId { get; set; }
        public string BotId { get; set; }
        public string BotName { get; set; }
        public string BotGroup { get; set; }
        public string StreamKey { get; set; }
        public string RTMP { get; set; }
        public string StreamName { get; set; }
        public LiveType LiveType { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public LiveOrder LiveOrder { get; set; }
        public bool IsForever { get; set; }
        public bool IsLiveNow { get; set; }
        public DateTime? StartTimeLive { get; set; }
        public DateTime? EndTimeLive { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? Updatedtime { get; set; }
        public StreamStatus Status { get; set; }
        public PlatformType PlatformType { get; set; }
        public RenderStatus RenderStatus { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class StreamAdminRequestDto : PageQuery
    {

    }

    public class StreamInfoUserDto
    {
        public string Live1080Name { get; set; }
        public string Live4kName { get; set; }
        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads1080InRun { get; set; }
        public int NumberOfThreads4K { get; set; }
        public int NumberOfThreads4KInRun { get; set; }
        public int NumberOfThreads1080Create { get; set; }
        public int NumberOfThreads1080CreateInRun { get; set; }
        public int NumberOfThreads4KCreateInRun { get; set; }
    }
    public class StreamReportDto
    {
        public int Total { get; set; }
        public int TotalCreated { get; set; }
        public int TotalStreaming { get; set; }
        public int TotalTimer { get; set; }
        public int TotalStop { get; set; }
    }
    public class StreamReportInfo
    {
        public StreamReportDto Live1080 { get; set; }
        public StreamReportDto Live4K { get; set; }
    }
    public class StreamLinkDto
    {
        public string Link { get; set; }
    }

    public class StreamReportAdminDto
    {
        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads1080InRun { get; set; }
        public int NumberOfThreads4K { get; set; }
        public int NumberOfThreads4KInRun { get; set; }
        public int TotalBOT { get; set; }
        public int TotalUser { get; set; }
        public int TotalManager { get; set; }
    }
}
