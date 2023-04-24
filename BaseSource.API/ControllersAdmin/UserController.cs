using BaseSouce.Services.Services.UserAdmin;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.UserAdmin;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("/api/admin/[controller]")]
    public class UserController : BaseAdminApiController
    {
        private readonly IUserAdminService _userAdminService;
        public UserController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        [HttpGet]
        [Route("/api/admin/users")]
        public async Task<IActionResult> GetAll([FromQuery] UserAdminRequestDto model)
        {
            var result = await _userAdminService.GetUserByFilterAsync(model);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpPost]
        [Route("manger")]
        public async Task<IActionResult> UpdateUserGroup(UserGroupAddDto model)
        {
            var result = await _userAdminService.UpdateUserGroupAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("managers")]
        public async Task<IActionResult> GetAllManager()
        {
            var result = await _userAdminService.GetUserManagerAsync();
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("manager/users")]
        public async Task<IActionResult> GetAllUserOfManager(string userId)
        {
            var result = await _userAdminService.GetAllUserOfManagerAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpPost]
        [Route("manager/role")]
        public async Task<IActionResult> UpdateRoleManager(string userName)
        {
            var result = await _userAdminService.UpdateManagerAsync(userName);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordAdminDto model)
        {
            var result = await _userAdminService.ResetPassowrdAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("/api/admin/user/managerBot")]
        public async Task<IActionResult> GetUserManagerBot([FromQuery] string userId)
        {
            var result = await _userAdminService.GetUserManagerBotAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpPost]
        [Route("/api/admin/user/managerBot/update")]
        public async Task<IActionResult> UpdateUserManagerBot([FromBody] UserManagerBotDto model)
        {
            var result = await _userAdminService.UpdateUserManagerBotAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
    }
}
