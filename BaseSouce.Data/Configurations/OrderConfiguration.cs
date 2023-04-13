using BaseSouce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BaseSouce.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.ServicePack).WithMany(x => x.Orders).HasForeignKey(x => x.ServicePackId);
    }
    }
}
