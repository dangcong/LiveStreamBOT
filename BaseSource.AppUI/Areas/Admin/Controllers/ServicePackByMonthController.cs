using BaseSource.ApiIntegration.WebApi.SerivePackage;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class ServicePackByMonthController : BaseAdminController
    {
        private readonly IServicePackageByMonthApiClient _servicePackageByMonthApiClient;
        public ServicePackByMonthController(IServicePackageByMonthApiClient servicePackageByMonthApiClient)
        {
            _servicePackageByMonthApiClient = servicePackageByMonthApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var result = await _servicePackageByMonthApiClient.GetServicePackByTypeAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.ServicePackId = id;
            return View(result.ResultObj);
        }
        [HttpGet]

        public IActionResult Create(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var model = new ServicePackByMonthCreateDto();
            model.ServicePackId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServicePackByMonthCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _servicePackageByMonthApiClient.CreateAsync(model);

            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Tạo gói tháng không thành công!");
                return View(model);
            }

            return Redirect("/admin");
        }


        [HttpGet]

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _servicePackageByMonthApiClient.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(ServicePackByMonthDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _servicePackageByMonthApiClient.UpdateAsync(model);

            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Cập nhật gói tháng không thành công!");
                return View(model);
            }

           return Redirect("/admin");
        }
    }
}
