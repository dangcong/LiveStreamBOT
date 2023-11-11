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
            var result = await _userAdminService.GetUserByFilterAsync(UserId, model, IsAdmin);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpGet]
        [Route("/api/admin/user/all")]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _userAdminService.GetAllAsync(UserId, IsAdmin);
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

        [HttpPost]
        [Route("telegram")]
        public async Task<IActionResult> UpdateTelegam(UpdateTelegramDto model)
        {
            var result = await _userAdminService.UpdateTelegramUserAsync(model, IsAdmin);
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
            model.UserUpdate = User.Identity?.Name;
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
            var result = await _userAdminService.GetUserBotAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }

        [HttpPost]
        [Route("/api/admin/user/managerBot/update")]
        public async Task<IActionResult> UpdateUserManagerBot([FromBody] UserManagerBotUpdateDto model)
        {
            var result = await _userAdminService.UpdateUserManagetBotAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }


        [HttpPost]
        [Route("/api/admin/user/managerBot/add")]
        public async Task<IActionResult> AddUserManagerBot([FromBody] UserAddBOTAdminDto model)
        {
            var result = await _userAdminService.InsertBOTOfUserAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("/api/admin/user/managerBot/delete/{id:int:min(1)}")]
        public async Task<IActionResult> DeleteUserManagerBOT(int id)
        {
            var result = await _userAdminService.DeleteUserManagerBot(id);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }


        [HttpPost]
        [Route("/api/admin/user/delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userAdminService.DeleteAsync(userId);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
        [HttpPost]
        [Route("/api/admin/user/create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateAdminDto model)
        {
            var result = await _userAdminService.CreateUserAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("/api/admin/user/addbot")]
        public async Task<IActionResult> AddBotToUser([FromBody] UserAddBotAdminDto model)
        {
            var result = await _userAdminService.AddUserToBotAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

    }
}
