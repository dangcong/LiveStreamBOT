using BaseSouce.Shared.Enums;

namespace BaseSouce.Data.Entities
{
    public class ServicePack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfThreads { get; set; }
        public int NumberOfThreadsCreate { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        public bool IsPublished { get; set; }
        public ServicePackType Type { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<ServicePackByMonth> ServicePackByMonths { get; set; }
    }
}
