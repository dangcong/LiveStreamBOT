using BaseSouce.Services.Services.StreamAdmin;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    public class StreamController : BaseAdminApiController
    {
        private readonly IStreamAdminService _streamAdminService;
        public StreamController(IStreamAdminService streamAdminService)
        {
            _streamAdminService = streamAdminService;
        }
        [HttpGet]
        [Route("/api/admin/streams")]
        public async Task<IActionResult> GetAll([FromQuery] StreamAdminRequestDto model)
        {
            var result = await _streamAdminService.GetByFilterAsync(model);
            return Ok(new ApiSuccessResult<object>(result));
        }
    }
}
