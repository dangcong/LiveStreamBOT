using BaseSouce.Shared.Enums;


namespace BaseSouce.Data.Entities
{
    public class ServicePack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfThreads { get; set; }
        public int MyProperty { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public ServicePackType Type { get; set; }
    }
}
