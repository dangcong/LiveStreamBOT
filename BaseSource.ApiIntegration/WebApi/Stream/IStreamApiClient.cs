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
        Task<ApiResult<string>> UpdateAsync(int id, StreamUpdateDto model);
        Task<ApiResult<string>> DeleteAsync(int id);
        Task<ApiResult<string>> StartStreamAsync(int id);
        Task<ApiResult<PagedResult<StreamClientInfoDto>>> GetByFilterAsync(StreamClientRequestDto model);
    }
}
