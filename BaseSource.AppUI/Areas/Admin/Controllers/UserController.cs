using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        [HttpGet]
        //[Route("/admin/User/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
