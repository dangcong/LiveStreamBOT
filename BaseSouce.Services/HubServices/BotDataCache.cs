using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;

namespace BaseSouce.Services.HubServices
{
    public interface IBotDataCache
    {
        ClientInfo? this[string key] { get; set; }
    }
    public class BotDataCache : IBotDataCache
    {
        readonly Dictionary<string, ClientInfo> ClientInfoDict = new Dictionary<string, ClientInfo>();

        public ClientInfo? this[string key]
        {
            get
            {
                if (ClientInfoDict.ContainsKey(key))
                {
                    return ClientInfoDict[key];
                }
                else return null;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(key) && value is not null)
                {
                    ClientInfoDict[key] = value;
                }
            }
        }
    }
}
