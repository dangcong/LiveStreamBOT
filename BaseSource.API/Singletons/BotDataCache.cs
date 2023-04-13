using BaseSouce.SharedSignalrData.Server;

namespace BaseSource.API.Singletons
{
    public interface IBotDataCache
    {
        ClientInfo? this[string key] { get; set; }
    }
    class BotDataCache : IBotDataCache
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
