namespace BaseSouce.SharedSignalrData.Server
{
    public class FileData
    {
        public long Size { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public UrlType UrlType { get; set; }
        public FileType FileType { get; set; }
    }
}
