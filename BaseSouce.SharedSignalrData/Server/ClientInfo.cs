namespace BaseSouce.SharedSignalrData.Server
{
    public class ClientInfo
    {
        /// <summary>
        /// Range: 0-1
        /// </summary>
        public double CpuUsage { get; set; } = 0;

        /// <summary>
        /// Range: 0-1
        /// </summary>
        public double RamUsage { get; set; } = 0;

        /// <summary>
        /// KBps
        /// </summary>
        public double Bandwidth { get; set; } = 0;
    }
}
