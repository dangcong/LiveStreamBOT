using BaseSouce.Services.Services.StreamAdmin;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("api/admin/[controller]")]
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
            var result = await _streamAdminService.GetByFilterAsync(UserId, model, IsAdmin);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _streamAdminService.GetStreamByIdAsync(id);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("bot/{id:int:min(1)}")]
        public async Task<IActionResult> GetAllByBotId(int id)
        {
            var result = await _streamAdminService.GetByBotIdAsync(id);
            return Ok(new ApiSuccessResult<object>(result));
        }


        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var result = await _streamAdminService.GetAllByUserIdAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("report")]
        public async Task<IActionResult> GetReportStream()
        {
            var result = await _streamAdminService.GetReportStreamAsync(UserId,IsAdmin);
            return Ok(new ApiSuccessResult<object>(result));

        }
    }
}
