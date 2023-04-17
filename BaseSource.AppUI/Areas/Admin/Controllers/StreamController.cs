using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class StreamController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
