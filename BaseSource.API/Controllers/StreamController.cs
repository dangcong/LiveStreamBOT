using BaseSouce.Services.Services.Stream;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{

    public class StreamController : BaseApiController
    {
        private readonly IStreamClientService _streamClientService;
        public StreamController(IStreamClientService streamClientService)
        {
            _streamClientService = streamClientService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(StreamCreateDto model)
        {
            var result = await _streamClientService.CreateAsync(UserId, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, [FromBody] StreamUpdateDto model)
        {
            var result = await _streamClientService.CreateAsync(UserId, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _streamClientService.DeleteAsync(UserId, id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var result = await _streamClientService.StartStreamAsync(UserId, id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("/api/streams")]
        public async Task<IActionResult> GetAll([FromQuery] StreamClientRequestDto model)
        {
            var result = await _streamClientService.GetByFilterAsync(UserId, model);
            return Ok(new ApiSuccessResult<object>(result));
        }
    }
}
