using BaseSouce.Shared.Enums;

namespace BaseSouce.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int ServicePackId { get; set; }
        public int Month { get; set; }
        public DateTime CreatedTime { get; set; }
        public double Price { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
