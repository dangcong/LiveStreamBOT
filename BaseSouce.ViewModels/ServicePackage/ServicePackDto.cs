using BaseSouce.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.ServicePackage
{
    public class ServicePackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfThreads { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsPublished { get; set; }

        public ServicePackType Type { get; set; }
    }

    public class ServicePackCreateDto
    {
        [Required(ErrorMessage ="Tên gói cước không được để trống")]
        public string Name { get; set; }
        [Range(1, int.MaxValue)]
        public int NumberOfThreads { get; set; }
        [Range(1, double.MaxValue,ErrorMessage ="Giá không hợp lệ")]
        public double Price { get; set; }
        [Range(0, double.MaxValue,ErrorMessage ="Giá không hợp lệ")]
        public double Discount { get; set; }

        public ServicePackType Type { get; set; }
    }

    public class ServicePackUpdateDto : ServicePackCreateDto
    {
        public int Id { get; set; }
    }
    public class InvoiceCodeInfoDto
    {
        public string UserName { get; set; }
        public string LinkFB { get; set; }
        public string PackageCurrent { get; set; }
        public DateTime? ExpiredPackageCurrent { get; set; }
        public string CodeBill { get; set; }
        public string PackageName { get; set; }
        public bool IsSuccessed { get; set; }
    }
}
