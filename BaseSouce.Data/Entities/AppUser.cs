using BaseSource.Data.Entities;
using BaseSource.Shared.Classes;
using BaseSource.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSouce.Data.Entities
{
    public class AppUser : IdentityUser<string>, IBotChatId
    {
        public string LinkFB { get; set; }
        public DateTime CreatedTime { get; set; }

        // username/userid split by ,
        public string TelegramUserId { get; set; }//TelegramAPI
        public string TelegramUserIdReceiveNotificationsFromStaff { get; set; }//TelegramBOT



        //public int NumberOfThreads1080 { get; set; }
        //public int NumberOfThreads4K { get; set; }
        public int NumberOfThreads1080Create { get; set; }
        public int NumberOfThreads4KCreate { get; set; }
        public DateTime? DeletedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UserUpdate { get; set; }
        public string UserManager { get; set; }

        [ForeignKey("UserManager")]

        public AppUser Manager { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }
        public ICollection<AppUserClaim> Claims { get; set; }
        public ICollection<AppUserToken> Tokens { get; set; }
        public ICollection<AppUserLogin> AppUserLogins { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<StreamHistory> StreamHistorys { get; set; }

        public ICollection<ManagerGroup> ManagerGroups { get; set; }
        public ICollection<ManagerGroup> UserGroups { get; set; }

        public ICollection<UserManagerBOT> UserManagerBOTs { get; set; }
        public ICollection<ManagerBOT> ManagerBOTs { get; set; }








        IEnumerable<string> IBotChatId.TelegramUserNames
        {
            get
            {
                return TelegramUserId?
                    .Split(',')?
                    .Select(x => x.Trim())?
                    .Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>();
            }
        }
        public IBotChatId GetBotChatIdForStaff()
        {
            return new BotChatId()
            {
                TelegramUserNames = TelegramUserIdReceiveNotificationsFromStaff?
                    .Split(',')?
                    .Select(x => x.Trim())?
                    .Where(x => !string.IsNullOrWhiteSpace(x))?
                    .ToList() ?? Enumerable.Empty<string>()
            };
        }
    }
}
