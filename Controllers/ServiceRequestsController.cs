using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly TechMoveApiClient _apiClient;

        public ServiceRequestsController(TechMoveApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _apiClient.GetServiceRequestsAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _apiClient.GetServiceRequestAsync(id.Value);

            return serviceRequest == null ? NotFound() : View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateContractsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Description,CostUSD,Status,ContractId")]
            ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var result =
                await _apiClient.CreateServiceRequestAsync(serviceRequest);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            TempData["SuccessMessage"] =
                "Service Request created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _apiClient.GetServiceRequestAsync(id.Value);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            await PopulateContractsAsync(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Description,CostUSD,Status,ContractId")]
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var result =
                await _apiClient.UpdateServiceRequestAsync(serviceRequest);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await PopulateContractsAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            TempData["SuccessMessage"] =
                "Service Request updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _apiClient.GetServiceRequestAsync(id.Value);

            return serviceRequest == null ? NotFound() : View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _apiClient.DeleteServiceRequestAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] =
                    "Service Request deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractsAsync(int? selectedContractId = null)
        {
            ViewData["ContractId"] = new SelectList(
                await _apiClient.GetContractsAsync(),
                "Id",
                "Status",
                selectedContractId);
        }
    }
}
