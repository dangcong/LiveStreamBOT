using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT
{
    internal static class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogsDir);
            Directory.CreateDirectory(WorkingFilesDir);
        }

        public static string ExeDir { get; } = Directory.GetCurrentDirectory();
        public static string LogsDir { get; } = Path.Combine(ExeDir, "Logs");
        public static string WorkingFilesDir { get; } = Path.Combine(ExeDir, "WorkingFiles");

    }
}
