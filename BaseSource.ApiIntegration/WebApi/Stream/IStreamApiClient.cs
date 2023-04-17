using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi.Stream
{
    public interface IStreamApiClient
    {
        Task<ApiResult<string>> Create(StreamCreateDto model);
    }
}
