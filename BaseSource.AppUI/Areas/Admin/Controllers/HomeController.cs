using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        [HttpGet]
        [Route("/admin")]
        public IActionResult Index()
        {
            return Redirect("/admin/user/index");
            //return View();
        }
    }
}
