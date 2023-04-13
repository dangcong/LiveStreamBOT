using BaseSouce.Data.Entities;
using BaseSouce.Services.ServicePackage;
using BaseSouce.Services.Services.Order;
using BaseSouce.Services.Services.OrderAdmin;
using BaseSouce.Services.Services.SendMail;
using BaseSouce.Services.Services.ServicePackage;
using BaseSouce.Services.Services.ServicePackageByMonth;
using BaseSouce.Services.Services.Setting;
using BaseSouce.Services.Services.User;
using Microsoft.AspNetCore.Identity;

namespace BaseSource.API.Cofigurations
{
    public static class DIConfigurations
    {
        public static IServiceCollection DIConfiguration(this IServiceCollection services)
        {
            //DI for user
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISendMailService, SendMailService>();

            services.AddTransient<IServicePackService, ServicePackService>();
            services.AddTransient<IServicePackByMonthService, ServicePackByMonthService>();
            services.AddTransient<IConfigSettingService, ConfigSettingService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderAdminService, OrderAdminService>();
            
            return services;
        }
    }
}
