using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Api.Repositories;

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<ServiceRequest>> GetAllAsync()
    {
        return _context.ServiceRequests
            .Include(s => s.Contract)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    public Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return _context.ServiceRequests
            .Include(s => s.Contract)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
    {
        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();
        return serviceRequest;
    }

    public async Task<bool> UpdateAsync(ServiceRequest serviceRequest)
    {
        if (!await _context.ServiceRequests.AnyAsync(s => s.Id == serviceRequest.Id))
        {
            return false;
        }

        _context.ServiceRequests.Update(serviceRequest);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(id);

        if (serviceRequest == null)
        {
            return false;
        }

        _context.ServiceRequests.Remove(serviceRequest);
        await _context.SaveChangesAsync();
        return true;
    }
}
