using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT
{
    internal static class LogHelper
    {
        static string LogsDir { get; } = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        static LogHelper()
        {
            Directory.CreateDirectory(LogsDir);
        }
        static readonly object _lock = new object();


        public static void WriteLog(string log)
        {
            log = $"{DateTime.Now:HH:mm:ss}\t{log}";
            Console.WriteLine(log);
            lock (_lock)
            {
                using StreamWriter streamWriter = new StreamWriter(Path.Combine(LogsDir, $"{DateTime.Now:yyyy-MM-dd}.log"), true);
                streamWriter.WriteLine(log);
            }
        }

    }
}
