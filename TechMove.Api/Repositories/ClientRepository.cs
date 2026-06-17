using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Api.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Client>> GetAllAsync()
    {
        return _context.Clients
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public Task<Client?> GetByIdAsync(int id)
    {
        return _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client> CreateAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> UpdateAsync(Client client)
    {
        if (!await _context.Clients.AnyAsync(c => c.Id == client.Id))
        {
            return false;
        }

        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            return false;
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}
