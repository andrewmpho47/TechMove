using TechMove.Models;

namespace TechMove.Api.Repositories;

public interface IContractRepository
{
    Task<List<Contract>> GetAllAsync(string? statusSearch, DateTime? startDate, DateTime? endDate);
    Task<Contract?> GetByIdAsync(int id);
    Task<Contract> CreateAsync(Contract contract);
    Task<bool> UpdateAsync(Contract contract);
    Task<bool> UpdateStatusAsync(int id, string status);
    Task<bool> DeleteAsync(int id);
}
