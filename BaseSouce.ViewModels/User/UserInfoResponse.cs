using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.ViewModels.User
{
    public class UserInfoResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }

    }

    /// <summary>
    /// <see cref="UserUpdateDto"/>
    /// </summary>
    public class ProfifleInfoDto
    {
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string LinkFB { get; set; }
        public string? TelegramUserId { get; set; }
        public string? DiscordUserId { get; set; }
        public string? TelegramUserIdReceiveNotificationsFromStaff { get; set; }
        public string? DiscordUserIdReceiveNotificationsFromStaff { get; set; }
        public PackageLiveInfoDto? PackageLive1080 { get; set; } = new PackageLiveInfoDto();
        public PackageLiveInfoDto? PackageLive4k { get; set; } = new PackageLiveInfoDto();
    }

    public class PackageLiveInfoDto
    {
        public string Name { get; set; }
        public int TotalBOT { get; set; }
        public int NumberOfThreads { get; set; }
        public int NumberOfThreadsInRun { get; set; }
        public int NumberOfThreadsCreated { get; set; }
        public int NumberOfThreadsCreatedInRun { get; set; }
        public int TotalPending { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int PercentNumberOfThreads
        {
            get
            {
                if (NumberOfThreads == 0)
                {
                    return 0;
                }
                return (int)(NumberOfThreadsInRun / NumberOfThreads * 100);
            }
            //get
            //{
            //    return 40;
            //}
        }
        public int PercentNumberOfThreadsCreated
        {
            //get
            //{
            //    return 30;
            //}
            get
            {
                if (NumberOfThreadsCreated == 0)
                {
                    return 0;
                }
                return (int)(NumberOfThreadsCreatedInRun / NumberOfThreadsCreated * 100);
            }
        }
    }

    public class PackageLiveUserDto
    {
        public PackageLiveInfoDto? PackageLive1080 { get; set; } = new PackageLiveInfoDto();
        public PackageLiveInfoDto? PackageLive4k { get; set; } = new PackageLiveInfoDto();
        public PackageLiveInfoDto? PackageLiveBackup { get; set; } = new PackageLiveInfoDto();
    }
}
