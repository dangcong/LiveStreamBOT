using AutoMapper;
using BaseSouce.Data.Entities;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Order;
using BaseSource.ViewModels.ServicePackage;
using BaseSource.ViewModels.Setting;

namespace BaseSouce.Services.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region ServicePack

            CreateMap<ServicePack, ServicePackDto>();
            CreateMap<ServicePackByMonth, ServicePackByMonthDto>();

            #endregion

            #region ConfigSystem
            CreateMap<ConfigSystem, ConfigSettingVm>();
            #endregion
            #region Order
            CreateMap<Data.Entities.Order, OrderInfoDto>()
                .ForMember(dest => dest.UserName, options => options.MapFrom(source => source.User.UserName));
            #endregion
        }

    }
}
