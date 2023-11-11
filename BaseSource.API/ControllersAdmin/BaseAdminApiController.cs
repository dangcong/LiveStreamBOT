using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.ControllersAdmin
{
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
   
    public class BaseAdminApiController : ControllerBase
    {
        private string _userId;
        private bool? _isAdmin;
        protected string UserId
        {
            get { return _userId ?? User.FindFirstValue(ClaimTypes.NameIdentifier); }
            set { _userId = value; }
        }
        protected bool IsAdmin
        {
            get { return User.IsInRole("Admin"); }
            set { _isAdmin = value; }
        }

        protected void AddErrors(string error, string property)
        {
            ModelState.AddModelError(property, error);
        }
    }
}
