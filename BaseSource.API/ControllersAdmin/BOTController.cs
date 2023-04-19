using BaseSouce.Services.Services.BOT;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("/api/admin/[controller]")]
    public class BOTController : BaseAdminApiController
    {
        private readonly IManagerBOTService _managerBOTService;
        public BOTController(IManagerBOTService managerBOTService)
        {
            _managerBOTService = managerBOTService;
        }
        [HttpGet]
        [Route("/api/admin/bots")]
        public async Task<IActionResult> GetAll([FromQuery] ManagerBOTRequestDto model)
        {
            var result = await _managerBOTService.GetBOTByFilterAsync(model);
            return Ok(new ApiSuccessResult<IPagedList<ManagerBotInfoDto>>(result));
        }

        [HttpPatch]
        [Route("{id:int:min(1)}/ChangeUsed")]
        public async Task<IActionResult> ChangeUsed(int id)
        {
            var result = await _managerBOTService.ChangeUsedAsync(id);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _managerBOTService.GetBOTByIdAsync(id);
            return Ok(new ApiSuccessResult<ManagerBotInfoDto>(result));
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, [FromBody] ManagerBotUpdateDto model)
        {
            var result = await _managerBOTService.UpdateBOTAsync(id, model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

    }
}
