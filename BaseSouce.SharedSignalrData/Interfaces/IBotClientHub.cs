using BaseSouce.SharedSignalrData.Classes;
namespace BaseSouce.SharedSignalrData.Interfaces
{
    public interface IBotClientHub
    {
        Task RenderAsync(RenderCommand renderCommand);
        Task SetSettingAsync(SettingInfo settingInfo);
    }
}
