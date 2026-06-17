using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardViewModel>> GetDashboard()
    {
        return Ok(new DashboardViewModel
        {
            TotalClients = await _context.Clients.CountAsync(),
            TotalContracts = await _context.Contracts.CountAsync(),
            TotalServiceRequests = await _context.ServiceRequests.CountAsync()
        });
    }
}
