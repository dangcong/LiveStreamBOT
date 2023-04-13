using BaseSource.ApiIntegration.WebApi;
using BaseSource.ApiIntegration.WebApi.Order;
using BaseSource.ApiIntegration.WebApi.OrderAdmin;
using BaseSource.ApiIntegration.WebApi.SerivePackage;
using BaseSource.ApiIntegration.WebApi.Setting;

namespace BaseSource.AppUI.Configuration
{
    public static class DIConfigurations
    {
        public static IServiceCollection DIConfiguration(this IServiceCollection services)
        {
            //DI for user
            //DI for user
            services.AddTransient<IUserApiClient, UserApiClient>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IServicePackageApiClient, ServicePackageApiClient>();
            services.AddTransient<IServicePackageByMonthApiClient, ServicePackageByMonthApiClient>();
            services.AddTransient<IConfigSettingApiClient, ConfigSettingApiClient>();

            services.AddTransient<IOrderApiClient, OrderApiClient>();

            services.AddTransient<IOrderAdminApiClient, OrderAdminApiClient>();

            
            return services;
        }
    }
}
