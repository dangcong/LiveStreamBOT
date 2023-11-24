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
using System.Security.Cryptography.X509Certificates;

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
                .ForMember(dest => dest.TotalUser,
                options => options.MapFrom(source => source.UserManagerBOTs.DistinctBy(x=>x.UserId).Count()))
                .ForMember(dest => dest.Manager,
                options => options.MapFrom(source => source.Manager.UserName))
                 .ForMember(dest => dest.UserManager,
                options => options.MapFrom(source => source.UserManager))
                .ForMember(dest => dest.NumberOfThreadsInRun,
                options => options.MapFrom(source => source.StreamHistorys.Where(x => x.Status == StreamStatus.Streaming).Count()));

            #endregion

            #region Stream History

            CreateMap<StreamHistory, StreamClientInfoDto>()
                .ForMember(dest => dest.BotBackupName, options => options.MapFrom(source => source.ManagerBOTBackup.Name))
                .ForMember(dest => dest.BackupGroup, options => options.MapFrom(source => source.ManagerBOTBackup.Group))
                .ForMember(dest => dest.BotName, options => options.MapFrom(source => source.ManagerBOT.Name))
                .ForMember(dest => dest.Group, options => options.MapFrom(source => source.ManagerBOT.Group));


            CreateMap<StreamHistory, StreamAdminInfoDto>()
                .ForMember(dest => dest.BotBackupName, options => options.MapFrom(source => source.ManagerBOTBackup.Name))
                .ForMember(dest => dest.BotBackupGroup, options => options.MapFrom(source => source.ManagerBOTBackup.Group))
               .ForMember(dest => dest.BotGroup, options => options.MapFrom(source => source.ManagerBOT.Group))
               .ForMember(dest => dest.BotName, options => options.MapFrom(source => source.ManagerBOT.Name))
               .ForMember(dest => dest.UserName, options => options.MapFrom(source => source.AppUser.UserName))
               .ForMember(dest => dest.UserManager, options => options.MapFrom(source => source.AppUser.Manager.UserName))
               .ForMember(dest => dest.Email, options => options.MapFrom(source => source.AppUser.Email));
            #endregion

            #region User
            
                CreateMap<AppUser, UserInfoResponse>();
            #endregion
        }

    }
}
