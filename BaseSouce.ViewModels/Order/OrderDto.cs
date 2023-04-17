

using BaseSouce.Shared.Enums;
using BaseSouce.ViewModels.Common;

namespace BaseSource.ViewModels.Order
{
    public class OrderCreateDto
    {
        public string CodeBill { get; set; }
        public int Month { get; set; }
        public string PackageName { get; set; }
    }
    public class OrderInfoDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int ServicePackId { get; set; }
        public string ServicePackName { get; set; }
        public int Month { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedTimeString
        {
            get
            {
                return CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public DateTime ExpiredTime { get; set; }
        public double Price { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public int NumberOfThreads { get; set; }
        public string UserName { get; set; }
        public ServicePackType Type { get; set; }

    }
    public class OrderRequestDto : PageQuery
    {

    }
}
