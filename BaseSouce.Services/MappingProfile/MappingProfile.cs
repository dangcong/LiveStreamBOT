using AutoMapper;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSouce.ViewModels.User;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.ManagerBOT;
using BaseSource.ViewModels.Order;
using BaseSource.ViewModels.ServicePackage;
using BaseSource.ViewModels.Setting;
using BaseSource.ViewModels.Stream;

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
            CreateMap<Order, OrderInfoDto>()
                .ForMember(dest => dest.UserName, options => options.MapFrom(source => source.User.UserName))
                .ForMember(dest => dest.ServicePackName, options => options.MapFrom(source => source.ServicePack.Name))
                .ForMember(dest => dest.Type, options => options.MapFrom(source => source.ServicePack.Type));
            #endregion

            #region ManagerBOT

            CreateMap<ManagerBOT, ManagerBotInfoDto>()
                .ForMember(dest => dest.NumberOfThreadsInRun,
                options => options.MapFrom(source => source.StreamHistorys.Where(x => x.Status == StreamStatus.Streaming).Count()));

            #endregion

            #region Stream History

            CreateMap<StreamHistory, StreamClientInfoDto>();


            CreateMap<StreamHistory, StreamAdminInfoDto>()
               .ForMember(dest => dest.UserName, options => options.MapFrom(source => source.AppUser.UserName))
               .ForMember(dest => dest.Email, options => options.MapFrom(source => source.AppUser.Email));
            #endregion

            #region User
            
                CreateMap<AppUser, UserInfoResponse>();
            #endregion
        }

    }
}
