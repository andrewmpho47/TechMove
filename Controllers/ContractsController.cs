using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly TechMoveApiClient _apiClient;

        public ContractsController(TechMoveApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index(
            string? statusSearch,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = await _apiClient.GetContractsAsync(
                statusSearch,
                startDate,
                endDate);

            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _apiClient.GetContractAsync(id.Value);

            return contract == null ? NotFound() : View(contract);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateClientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,StartDate,EndDate,Status,ServiceLevel,AgreementFilePath,ClientId")]
            Contract contract,
            IFormFile? signedAgreement)
        {
            if (signedAgreement != null)
            {
                var extension =
                    Path.GetExtension(signedAgreement.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            if (signedAgreement != null)
            {
                contract.AgreementFilePath =
                    await SaveSignedAgreementAsync(signedAgreement);
            }

            var result = await _apiClient.CreateContractAsync(contract);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            TempData["SuccessMessage"] = "Contract created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _apiClient.GetContractAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            await PopulateClientsAsync(contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StartDate,EndDate,Status,ServiceLevel,AgreementFilePath,ClientId")]
            Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            var result = await _apiClient.UpdateContractAsync(contract);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                await PopulateClientsAsync(contract.ClientId);
                return View(contract);
            }

            TempData["SuccessMessage"] = "Contract updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var result = await _apiClient.UpdateContractStatusAsync(id, status);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Contract status updated.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _apiClient.GetContractAsync(id.Value);

            return contract == null ? NotFound() : View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _apiClient.DeleteContractAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Contract deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateClientsAsync(int? selectedClientId = null)
        {
            ViewData["ClientId"] = new SelectList(
                await _apiClient.GetClientsAsync(),
                "Id",
                "Name",
                selectedClientId);
        }

        private static async Task<string> SaveSignedAgreementAsync(
            IFormFile signedAgreement)
        {
            var uploadsFolder =
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName =
                Guid.NewGuid() + "_" + signedAgreement.FileName;

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var fileStream =
                new FileStream(filePath, FileMode.Create);

            await signedAgreement.CopyToAsync(fileStream);

            return "/uploads/" + uniqueFileName;
        }
    }
}
