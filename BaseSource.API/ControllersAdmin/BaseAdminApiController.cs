using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.ControllersAdmin
{
    [ApiController]
    [Authorize(Roles = "Admin")]
   
    public class BaseAdminApiController : ControllerBase
    {
        private string _userId;
        protected string UserId
        {
            get { return _userId ?? User.FindFirstValue(ClaimTypes.NameIdentifier); }
            set { _userId = value; }
        }

        protected void AddErrors(string error, string property)
        {
            ModelState.AddModelError(property, error);
        }
    }
}
