using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStreamBOT.Helpers
{
    internal static class ClientInfoHelper
    {
        internal static async Task<ClientInfo> GetClientInfo()
        {
            var clientInfo = new ClientInfo();
            clientInfo.CpuUsage = GetPercentCpu();
            clientInfo.RamUsage = await GetPercentRamAsync();
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
                    if (int.TryParse(x, out int r)) return r;
                    return new Nullable<int>();
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();
            double idle = numColumns.Skip(3).First();

            return idle / numColumns.Sum() / (cpuLineCount - 1);
        }

        static async Task<double> GetPercentRamAsync()
        {
            IEnumerable<string> lines = await ShellExecuteAsync("/usr/bin/free");

            string lineMem = lines.First(x => x.StartsWith("Mem:"));
            var numColumns = lineMem
                .Split(' ')
                .Select(x =>
                {
                    if (int.TryParse(x, out int r)) return r;
                    return new Nullable<int>();
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();

            return numColumns[1] * 1.0 / numColumns[0];
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
