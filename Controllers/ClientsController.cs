using Microsoft.AspNetCore.Mvc;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ClientsController : Controller
    {
        private readonly TechMoveApiClient _apiClient;

        public ClientsController(TechMoveApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _apiClient.GetClientsAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _apiClient.GetClientAsync(id.Value);

            return client == null ? NotFound() : View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var result = await _apiClient.CreateClientAsync(client);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(client);
            }

            TempData["SuccessMessage"] = "Client created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _apiClient.GetClientAsync(id.Value);

            return client == null ? NotFound() : View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(client);
            }

            var result = await _apiClient.UpdateClientAsync(client);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage!);
                return View(client);
            }

            TempData["SuccessMessage"] = "Client updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _apiClient.GetClientAsync(id.Value);

            return client == null ? NotFound() : View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _apiClient.DeleteClientAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Client deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
