using LiveStreamBOT.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>RuntimeIdentifier của tiến trình hiện tại (win-x64, linux-x64...), dùng cho auto-update per-RID.</summary>
        public static string Rid { get; } = GetRuntimeIdentifier();

        static string GetRuntimeIdentifier()
        {
            string os =
                OperatingSystem.IsWindows() ? "win" :
                OperatingSystem.IsLinux() ? "linux" :
                OperatingSystem.IsMacOS() ? "osx" :
                "unknown";
            string arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "arm64",
                Architecture.Arm => "arm",
                _ => "x64",
            };
            return $"{os}-{arch}";
        }
    }
}
