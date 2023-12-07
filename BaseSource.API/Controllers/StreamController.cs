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
        [HttpPost]

        [Route("/api/stream")]
        //[AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] StreamCreateDto model)
        {


            // var result = await _streamClientService.CreateAsync(UserId ?? "7c533d74-3eed-4b51-a03d-cbe4b130ce9b", model);

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
            var result = await _streamClientService.UpdateAsync(UserId, id, model);
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

        [HttpPost]
        //[AllowAnonymous]
        [Route("{id:int:min(1)}/start")]
        public async Task<IActionResult> Start(int id)
        {
            //var result = await _streamClientService.StartStreamAsync(UserId??"7c533d74-3eed-4b51-a03d-cbe4b130ce9b", id);
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

        [HttpPost]
        //  [AllowAnonymous]
        [Route("{id:int:min(1)}/stop")]
        public async Task<IActionResult> Stop(int id)
        {
            // var result = await _streamClientService.CancelStreamAsync(UserId ?? "7c533d74-3eed-4b51-a03d-cbe4b130ce9b", id);
            var result = await _streamClientService.CancelStreamAsync(UserId, id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _streamClientService.GetStreamByIdAsync(UserId, id);
            if (result == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("report")]
        public async Task<IActionResult> GetReportByUser()
        {
            var result = await _streamClientService.GetStreamReportAsync(UserId);
            return Ok(new ApiSuccessResult<StreamReportDto>(result));
        }

        [HttpPost]
        [Route("validate-link")]
        public async Task<IActionResult> ValidateLink([FromBody] StreamLinkDto model)
        {
            var result = await _streamClientService.ValidateLinkAsync(model.Link);
            if (!result.Key)
            {
                return Ok(new ApiErrorResult<string>(result.Value));
            }
            return Ok(new ApiSuccessResult<object>(string.Empty));
        }

        [HttpGet]
        [Route("/api/stream/bots")]
        public async Task<IActionResult> GetBotsOfUser()
        {
            var result = await _streamClientService.GetBotOfUserAsync(UserId);
            return Ok(new ApiSuccessResult<object>(result));
        }
        [HttpGet]
        [Route("/api/stream/botsOfUser")]
        public async Task<IActionResult> GetBotsOfUser(string userId)
        {
            var result = await _streamClientService.GetBotOfUserAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpPost]
        [Route("/api/stream/update/botbackup")]
        public async Task<IActionResult> UpdateBotBackup([FromBody] StreamBackupUpdateDto model)
        {
            var result = await _streamClientService.UpdateBotBackupAsync(UserId, model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
    }
}
