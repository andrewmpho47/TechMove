using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceRequestsController(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext =
                _context.ServiceRequests.Include(s => s.Contract);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        public IActionResult Create()
        {
            ViewData["ContractId"] =
                new SelectList(_context.Contracts, "Id", "Status");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Description,CostUSD,CostZAR,Status,ContractId")]
            ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract != null)
            {
                var contractStatus = contract.Status?.Trim().ToLower();

                if (contractStatus == "expired" ||
                    contractStatus == "on hold")
                {
                    ModelState.AddModelError(
                        "",
                        "Service Request cannot be created for Expired or On Hold contracts.");
                }
            }

            if (ModelState.IsValid)
            {
                serviceRequest.CostZAR =
                    await ConvertUsdToZar(serviceRequest.CostUSD);

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Service Request created successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] =
                new SelectList(
                    _context.Contracts,
                    "Id",
                    "Status",
                    serviceRequest.ContractId);

            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest =
                await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            ViewData["ContractId"] =
                new SelectList(
                    _context.Contracts,
                    "Id",
                    "Status",
                    serviceRequest.ContractId);

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Description,CostUSD,CostZAR,Status,ContractId")]
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    serviceRequest.CostZAR =
                        await ConvertUsdToZar(serviceRequest.CostUSD);

                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Service Request updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] =
                new SelectList(
                    _context.Contracts,
                    "Id",
                    "Status",
                    serviceRequest.ContractId);

            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest =
                await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Service Request deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(
                "https://open.er-api.com/v6/latest/USD");

            if (!response.IsSuccessStatusCode)
            {
                return usdAmount * 18;
            }

            var json = await response.Content.ReadAsStringAsync();

            var exchangeRateResponse =
                JsonSerializer.Deserialize<ExchangeRateResponse>(json);

            if (exchangeRateResponse == null ||
                !exchangeRateResponse.Rates.ContainsKey("ZAR"))
            {
                return usdAmount * 18;
            }

            var zarRate = exchangeRateResponse.Rates["ZAR"];

            return usdAmount * zarRate;
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}