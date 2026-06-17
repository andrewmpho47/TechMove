using Microsoft.AspNetCore.Mvc;
using TechMove.Api.Dtos;
using TechMove.Api.Repositories;
using TechMove.Models;

namespace TechMove.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clients;

    public ClientsController(IClientRepository clients)
    {
        _clients = clients;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Client>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Client>>> GetClients()
    {
        return Ok(await _clients.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _clients.GetByIdAsync(id);
        return client == null ? NotFound() : Ok(client);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Client>> CreateClient(ClientRequest request)
    {
        var client = new Client
        {
            Name = request.Name,
            ContactDetails = request.ContactDetails,
            Region = request.Region
        };

        var created = await _clients.CreateAsync(client);
        return CreatedAtAction(nameof(GetClient), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClient(int id, ClientRequest request)
    {
        var client = new Client
        {
            Id = id,
            Name = request.Name,
            ContactDetails = request.ContactDetails,
            Region = request.Region
        };

        return await _clients.UpdateAsync(client) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(int id)
    {
        return await _clients.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
