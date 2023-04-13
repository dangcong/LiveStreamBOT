

namespace BaseSouce.Data.Entities
{
    public class ServicePackByMonth
    {
        public int Id { get; set; }
        public int ServicePackId { get; set; }
        public int Month { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double Percent { get; set; }

        public ServicePack ServicePack { get; set; }
    }
}
