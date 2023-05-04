using BaseSouce.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ViewModels.UserAdmin
{
    public class UserAdminInfoDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LinkFB { get; set; }
        public string LinkTelegram { get; set; }
        public DateTime CreatedTime { get; set; }
        public string TelegramAPI { get; set; }

        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads4K { get; set; }
        public int NumberOfThreads1080InRun { get; set; }
        public int NumberOfThreads4KInRun { get; set; }
        public string Password { get; set; }
    }
    public class UserAdminRequestDto:PageQuery
    {
        public string UserName { get; set; }
    }
    public class UserGroupDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserManagerId { get; set; }
    }
    public class UserGroupAddDto
    {
        public string UserName { get; set; }
        public string UserManagerId { get; set; }
    }
    public class UserGroupDeleteDto : UserGroupAddDto
    {

    }
    public class ResetPasswordAdminDto
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class UserManagerBotDto
    {
        public string UserId { get; set; }
        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads4K { get; set; }
        public int? Bot1080 { get; set; }
        public int? Bot4K { get; set; }
    }
    public class UserCreateAdminDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads4K { get; set; }
        public string LinkTelegram { get; set; }
    }
}
