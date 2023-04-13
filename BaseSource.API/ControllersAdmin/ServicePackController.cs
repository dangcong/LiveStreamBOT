using BaseSouce.Services.ServicePackage;
using BaseSouce.Shared.Enums;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("api/admin/[controller]")]
    public class ServicePackController : BaseAdminApiController
    {
        private readonly IServicePackService _servicePackService;
        public ServicePackController(IServicePackService servicePackService)
        {
            _servicePackService = servicePackService;
        }

        [HttpGet]
        [Route("id/{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _servicePackService.GetByIdAsync(id);
            return Ok(new ApiSuccessResult<ServicePackDto>(result));
        }

        [HttpGet]
        [Route("{type:int:min(1)}")]
        public async Task<IActionResult> GetServicePackByType(ServicePackType type)
        {
            var result = await _servicePackService.GetServicePackByTypeAsync(type);
            return Ok(new ApiSuccessResult<List<ServicePackDto>>(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServicePackCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _servicePackService.CreateAsync(model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> UpdateArticle([FromRoute] int id, [FromBody] ServicePackUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _servicePackService.UpdateAsync(id, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            var result = await _servicePackService.DeleteAsync(id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }

    }
}
