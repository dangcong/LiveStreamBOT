using BaseSource.Shared.Constants;
using BaseSource.AppUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using BaseSource.ApiIntegration.WebApi.SerivePackage;

namespace BaseSource.AppUI.Controllers
{


    public class HomeController : BaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IServicePackageApiClient _servicePackageApiClient;
        private readonly IServicePackageByMonthApiClient _servicePackageByMonthApiClient;
        public HomeController(IServicePackageApiClient servicePackageApiClient, IServicePackageByMonthApiClient servicePackageByMonthApiClient)
        {
            _servicePackageApiClient = servicePackageApiClient;
            _servicePackageByMonthApiClient = servicePackageByMonthApiClient;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return Redirect("/admin");
            }
            return Redirect("/stream/create");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[Route("ServicePack")]
        //[HttpGet]
        //public IActionResult ServicePack()
        //{
        //    var packs= _servicePackageApiClient.GetServicePackByTypeAsync()
        //    return View();
        //}

        [HttpGet]
        [Route("/invoice/{live}")]
        public IActionResult Invoice(string live)
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}