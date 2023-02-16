using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveStreamBOT.Helpers
{
    /// <summary>
    /// https://wiki.tino.org/docs/cac-tap-lenh-co-ban-cua-thu-muc-proc-trong-linux/
    /// cpu /proc/stat
    /// ram /proc/meminfo
    /// </summary>
    internal static class ClientInfoHelper
    {
        internal static async Task<ClientInfo> GetClientInfo()
        {
            var clientInfo = new ClientInfo();
            clientInfo.CpuUsage = GetPercentCpu();
            clientInfo.RamUsage = GetPercentRam();
            return clientInfo;
        }


        static double GetPercentCpu()
        {
            //https://www.baeldung.com/linux/get-cpu-usage#2-getting-cpu-usage-using-procstat
            var lines = File.ReadAllLines("/proc/stat", Encoding.ASCII);
            int cpuLineCount = lines.Count(x => x.StartsWith("cpu"));

            var numColumns = lines
                .First(x => x.StartsWith("cpu "))
                .Split(' ')
                .Select(x =>
                {
                    if (double.TryParse(x.Trim(), out double r)) return r;
                    return double.NaN;
                })
                .Where(x => !double.IsNaN(x))
                .ToArray();
            double idle = numColumns.Skip(3).FirstOrDefault();//default is 0
            return idle / numColumns.Sum() / (cpuLineCount - 1);//div 0 -> infinity
        }

        static double GetPercentRam()
        {
            var lines = File.ReadAllLines("/proc/meminfo", Encoding.ASCII);

            string lineMemTotal = lines.FirstOrDefault(x => x.StartsWith("MemTotal:"));
            string lineMemFree = lines.FirstOrDefault(x => x.StartsWith("MemFree:"));

            double memTotal = GetNumValue(lineMemTotal);
            double memFree = GetNumValue(lineMemFree);

            return (memTotal - memFree) / memTotal;
        }

        static double GetBandwidth()
        {

            return double.NaN;
        }




        static readonly Regex regex_num = new Regex("\\d+");
        static double GetNumValue(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                Match match = regex_num.Match(text);
                if (match.Success)
                {
                    return double.Parse(match.Value);
                }
            }
            return double.NaN;
        }

        static async Task<IEnumerable<string>> ShellExecuteAsync(string filename, string args = null, bool isShell = false)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = filename;
            processStartInfo.Arguments = args;
            processStartInfo.UseShellExecute = isShell;
            processStartInfo.RedirectStandardOutput = true;

            using var process = Process.Start(processStartInfo);

            string output = await process.StandardOutput.ReadToEndAsync();

            return output.Split(new char[] { '\r', '\n' })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
    }
}
