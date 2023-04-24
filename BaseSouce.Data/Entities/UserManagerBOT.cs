using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;

namespace BaseSource.Data.Entities
{
    public class UserManagerBOT
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ManagerBOTId { get; set; }
        public LiveType LiveType { get; set; }
        public DateTime CreatedTime { get; set; }

        public AppUser User { get; set; }
        public ManagerBOT ManagerBOT { get; set; }
    }
}
