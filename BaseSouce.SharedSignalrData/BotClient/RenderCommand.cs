using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData.Server
{
    public class RenderCommand : Identity
    {
        public string Arguments { get; set; }
        public List<FileData> FileDatas { get; set; }
        public Command Command { get; set; }
        public FileType FileType { get; set; }
    }
}
