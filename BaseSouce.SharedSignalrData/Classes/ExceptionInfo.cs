namespace BaseSouce.SharedSignalrData.Classes
{
    public class ExceptionInfo
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public string StackTrace { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Message}, {StackTrace}";
        }
    }
}
