using LiveStreamBOT.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveStreamBOT.Services
{
    /// <summary>
    /// https://wiki.tino.org/docs/cac-tap-lenh-co-ban-cua-thu-muc-proc-trong-linux/
    /// cpu /proc/stat
    /// ram /proc/meminfo
    /// </summary>
    internal class LinuxSystemInfoReader : ISystemInfoReader
    {
        public void Dispose()
        {

        }



        double old_Transmit = double.NaN;
        DateTime old_TransmitTime = DateTime.Now;
        public double GetBandwidth()
        {
            var lines = File.ReadAllLines("/proc/net/dev", Encoding.ASCII).Select(x => x.Trim()).ToArray();

            var mainLine = lines.FirstOrDefault(x => x.StartsWith(Singleton.Setting.Setting.NetworkName));
            if (string.IsNullOrWhiteSpace(mainLine)) 
                return double.NaN;

            var numColumns = mainLine
                .Split(' ')
                .Select(x => GetNumValue(x.Trim()))
                .Where(x => !double.IsNaN(x))
                .ToArray();

            if (double.IsNaN(old_Transmit))
            {
                old_Transmit = numColumns.Skip(9).FirstOrDefault();
                old_TransmitTime = DateTime.Now;
                return double.NaN;
            }
            else
            {
                double new_Transmit = numColumns.Skip(9).FirstOrDefault();
                DateTime new_TransmitTime = DateTime.Now;

                double byte_Transmit = new_Transmit - old_Transmit;
                double time_Transmit = (new_TransmitTime - old_TransmitTime).TotalSeconds;

                old_Transmit = new_Transmit;
                old_TransmitTime = new_TransmitTime;

                return byte_Transmit / time_Transmit;
            }
        }

        public double GetPercentCpu()
        {
            //https://www.baeldung.com/linux/get-cpu-usage#2-getting-cpu-usage-using-procstat
            var lines = File.ReadAllLines("/proc/stat", Encoding.ASCII).Select(x => x.Trim()).ToArray();
            int cpuLineCount = lines.Count(x => x.StartsWith("cpu"));

            var numColumns = lines
                .First(x => x.StartsWith("cpu "))
                .Split(' ')
                .Select(x => GetNumValue(x.Trim()))
                .Where(x => !double.IsNaN(x))
                .ToArray();
            double idle = numColumns.Skip(3).FirstOrDefault();//default is 0
            return idle / numColumns.Sum() / (cpuLineCount - 1);//div 0 -> infinity
        }

        public double GetPercentRam()
        {
            var lines = File.ReadAllLines("/proc/meminfo", Encoding.ASCII).Select(x => x.Trim()).ToArray();

            string lineMemTotal = lines.FirstOrDefault(x => x.StartsWith("MemTotal:"));
            string lineMemFree = lines.FirstOrDefault(x => x.StartsWith("MemFree:"));

            double memTotal = GetNumValue(lineMemTotal);
            double memFree = GetNumValue(lineMemFree);

            return (memTotal - memFree) / memTotal;
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
    }
}
