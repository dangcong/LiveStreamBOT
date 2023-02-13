using LiveStreamBOT.DataClass;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LiveStreamBOT
{
    internal static class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogsDir);
            Directory.CreateDirectory(BaseWorkingDir);
        }

        public static string ExeDir { get; } = Directory.GetCurrentDirectory();
        public static string LogsDir { get; } = Path.Combine(ExeDir, "Logs");
        public static string BaseWorkingDir { get; } = Path.Combine(ExeDir, "BaseWorkingDir");
        public static SaveSettingData<SettingData> Setting { get; } = new SaveSettingData<SettingData>(Path.Combine(ExeDir, "Setting.json"));
    }
}
