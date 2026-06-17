using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class HomeController : Controller
    {
        private readonly TechMoveApiClient _apiClient;

        public HomeController(TechMoveApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _apiClient.GetDashboardAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
