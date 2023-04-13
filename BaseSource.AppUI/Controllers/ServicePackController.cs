using BaseSouce.Shared.Enums;
using BaseSource.ApiIntegration.WebApi.SerivePackage;
using BaseSource.ApiIntegration.WebApi.Setting;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace BaseSource.AppUI.Controllers
{
    public class ServicePackController : BaseController
    {
        private readonly IServicePackageApiClient _servicePackageApiClient;
        private readonly IServicePackageByMonthApiClient _servicePackageByMonthApiClient;
        private readonly IConfigSettingApiClient _configSettingApiClient;
        public ServicePackController(IServicePackageApiClient servicePackageApiClient
            , IServicePackageByMonthApiClient servicePackageByMonthApiClient
            , IConfigSettingApiClient configSettingApiClient)
        {
            _servicePackageApiClient = servicePackageApiClient;
            _servicePackageByMonthApiClient = servicePackageByMonthApiClient;
            _configSettingApiClient = configSettingApiClient;
        }

        [Route("live/pricing")]
        [HttpGet]
        public async Task<IActionResult> ServicePack(ServicePackType type)
        {
            var packs = await _servicePackageApiClient.GetServicePackByTypeAsync(ServicePackType._1080);
            return View(packs?.ResultObj);
        }

        [Route("live-4k/pricing")]
        [HttpGet]
        public async Task<IActionResult> ServicePack4k()
        {
            var packs = await _servicePackageApiClient.GetServicePackByTypeAsync(ServicePackType._4K);
            return View(packs.ResultObj);
        }


        [HttpGet]
        [Route("/live/invoice/{live}")]
        public async Task<IActionResult> Invoice(string live)
        {
            var packsByMonths = await _servicePackageByMonthApiClient.GetServicePackByNameAsync(live);
            if (packsByMonths.IsSuccessed)
            {
                var setting = await _configSettingApiClient.GetSetting();

                ViewBag.Setting = setting.ResultObj;

                var invoiceInfo = await _servicePackageApiClient.GetInvoiceInfoAsync(live);

                if (invoiceInfo==null || !invoiceInfo.ResultObj.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.InvoiceInfo = invoiceInfo.ResultObj;

                return View(packsByMonths.ResultObj);
            }
            return NotFound();
        }


    }
}
