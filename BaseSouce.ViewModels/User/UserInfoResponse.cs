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
        public DateTime JoinedDate { get; set; }
        public double WalletBalance { get; set; }
        public string CurrencyFormat { get; set; }
        public bool Verification { get; set; }
        public string DocumentFront { get; set; }
        public string DocumentBack { get; set; }
        public string DocumentNumber { get; set; }
        public string ProofImage { get; set; }
        public DateTime CreatedTime { get; set; }
        public double WalletBonus { get; set; }
        public string ReferralCode { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }

    public class ProfifleInfoDto
    {
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string LinkFB { get; set; }
        public PackageLiveInfoDto? PackageLive1080 { get; set; } = new PackageLiveInfoDto();
        public PackageLiveInfoDto? PackageLive4k { get; set; } = new PackageLiveInfoDto();
    }

    public class PackageLiveInfoDto
    {
        public int NumberOfThreads { get; set; }
        public int NumberOfThreadsInRun { get; set; }
        public int NumberOfThreadsCreated { get; set; }
        public int NumberOfThreadsCreatedInRun { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
