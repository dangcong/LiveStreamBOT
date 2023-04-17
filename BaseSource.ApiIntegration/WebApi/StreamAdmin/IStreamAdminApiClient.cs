using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi.StreamAdmin
{
    public interface IStreamAdminApiClient
    {
       Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByFilterAsync(StreamAdminRequestDto model);
    }
}
