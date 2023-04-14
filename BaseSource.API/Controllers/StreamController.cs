using BaseSouce.Services.Services.Stream;
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
            var result =await _streamClientService.CreateAsync(null, model);
            if (result.Key)
            {
                return Ok();
            }
            return BadRequest(result.Value);
        }
    }
}
