using BaseSource.ViewModels.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.Stream
{
    public interface IStreamClientService
    {
        Task<KeyValuePair<bool, string>> CreateAsync(string userId, StreamCreateDto model);
    }
}
