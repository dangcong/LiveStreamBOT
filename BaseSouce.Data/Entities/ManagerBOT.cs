using BaseSouce.Shared.Enums;
using BaseSource.Data.Entities;

namespace BaseSouce.Data.Entities
{
    public class ManagerBOT
    {
        public int Id { get; set; }
        public string BotId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public ManagerBOTStatus Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int Retry { get; set; }

        public ICollection<StreamHistory > StreamHistorys { get; set; }
    }
}
