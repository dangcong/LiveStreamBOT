using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    public class BOTController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
