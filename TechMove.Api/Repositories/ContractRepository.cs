using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Api.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _context;

    public ContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Contract>> GetAllAsync(
        string? statusSearch,
        DateTime? startDate,
        DateTime? endDate)
    {
        var contracts = _context.Contracts
            .Include(c => c.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusSearch))
        {
            contracts = contracts.Where(c => c.Status.Contains(statusSearch));
        }

        if (startDate.HasValue)
        {
            contracts = contracts.Where(c => c.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var inclusiveEndDate = endDate.Value.Date.AddDays(1);
            contracts = contracts.Where(c => c.EndDate < inclusiveEndDate);
        }

        return contracts
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();
    }

    public Task<Contract?> GetByIdAsync(int id)
    {
        return _context.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contract> CreateAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task<bool> UpdateAsync(Contract contract)
    {
        if (!await _context.Contracts.AnyAsync(c => c.Id == contract.Id))
        {
            return false;
        }

        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var contract = await _context.Contracts.FindAsync(id);

        if (contract == null)
        {
            return false;
        }

        contract.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);

        if (contract == null)
        {
            return false;
        }

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();
        return true;
    }
}
