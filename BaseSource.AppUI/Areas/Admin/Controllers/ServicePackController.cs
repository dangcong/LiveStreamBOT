using BaseSouce.Shared.Enums;
using BaseSource.ApiIntegration.WebApi.SerivePackage;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class ServicePackController : BaseAdminController
    {
        private readonly IServicePackageApiClient _servicePackageApiClient;
        private readonly IServicePackageByMonthApiClient _servicePackageByMonthApiClient;
        public ServicePackController(IServicePackageApiClient servicePackageApiClient
            , IServicePackageByMonthApiClient servicePackageByMonthApiClient)
        {
            _servicePackageApiClient = servicePackageApiClient;
            _servicePackageByMonthApiClient = servicePackageByMonthApiClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        //[Route("all-servicePacks")]
        //[Route("/admin/all-servicePacks")]
        public IActionResult All()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Live()
        {
            var packs = await _servicePackageApiClient.GetServicePackByTypeAsync(ServicePackType._1080);
            return View(packs?.ResultObj);
        }
        [HttpGet]
        public async Task<IActionResult> Live4k()
        {
            var packs = await _servicePackageApiClient.GetServicePackByTypeAsync(ServicePackType._4K);
            return View(packs?.ResultObj);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {

            return View(new ServicePackCreateDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServicePackCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _servicePackageApiClient.CreateAsync(model);

            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Tạo gói không thành công!");
                return View(model);
            }

            if (model.Type == ServicePackType._4K)
            {
                return RedirectToAction("Live4k");
            }
            return RedirectToAction("Live");
        }


        [HttpGet]

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _servicePackageApiClient.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(ServicePackUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _servicePackageApiClient.UpdateAsync(model.Id, model);

            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Cập nhật gói không thành công!");
                return View(model);
            }

            if (model.Type == ServicePackType._4K)
            {
                return RedirectToAction("Live4k");
            }
            return RedirectToAction("Live");
        }
    }
}
