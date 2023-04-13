namespace BaseSource.ViewModels.ServicePackage
{
    public class ServicePackByMonthDto
    {
        public int Id { get; set; }
        public int ServicePackId { get; set; }
        public int Month { get; set; }
        public double Price { get; set; }
        public double Discount
        {
            get
            {
                if (Percent == 0)
                {
                    return Price;
                }
                return Price - (Price * Percent) / 100;

            }
        }
        public double Percent { get; set; }
        public string ServicePackName { get; set; }
    }
    public class ServicePackByMonthCreateDto
    {
        public int ServicePackId { get; set; }
        public int Month { get; set; }
        public double Price { get; set; }
        public double Discount
        {
            get
            {
                if (Percent == 0)
                {
                    return Price;
                }
                return Price - (Price * Percent) / 100;

            }
        }
        public double Percent { get; set; }
    }
    public class ServicePackByMonthUpdateDto : ServicePackByMonthCreateDto
    {
    }
}
