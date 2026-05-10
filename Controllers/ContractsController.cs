using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string statusSearch,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusSearch))
            {
                contracts = contracts.Where(c =>
                    c.Status.Contains(statusSearch));
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1);

                contracts = contracts.Where(c =>
                    c.EndDate < inclusiveEndDate);
            }

            return View(await contracts.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public IActionResult Create()
        {
            ViewData["ClientId"] =
                new SelectList(_context.Clients, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,StartDate,EndDate,Status,ServiceLevel,AgreementFilePath,ClientId")]
            Contract contract,
            IFormFile signedAgreement)
        {
            if (signedAgreement != null)
            {
                var extension =
                    Path.GetExtension(signedAgreement.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError(
                        "",
                        "Only PDF files are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (signedAgreement != null)
                {
                    var uploadsFolder =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/uploads");

                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName =
                        Guid.NewGuid().ToString() + "_" +
                        signedAgreement.FileName;

                    var filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream =
                           new FileStream(filePath, FileMode.Create))
                    {
                        await signedAgreement.CopyToAsync(fileStream);
                    }

                    contract.AgreementFilePath =
                        "/uploads/" + uniqueFileName;
                }

                _context.Add(contract);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contract created successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] =
                new SelectList(
                    _context.Clients,
                    "Id",
                    "Name",
                    contract.ClientId);

            return View(contract);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] =
                new SelectList(
                    _context.Clients,
                    "Id",
                    "Name",
                    contract.ClientId);

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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Contract updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] =
                new SelectList(
                    _context.Clients,
                    "Id",
                    "Name",
                    contract.ClientId);

            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contract deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}