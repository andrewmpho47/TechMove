using Microsoft.AspNetCore.Mvc;
using TechMove.Api.Dtos;
using TechMove.Api.Repositories;
using TechMove.Models;

namespace TechMove.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractRepository _contracts;

    public ContractsController(IContractRepository contracts)
    {
        _contracts = contracts;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Contract>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Contract>>> GetContracts(
        [FromQuery] string? statusSearch,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var contracts = await _contracts.GetAllAsync(statusSearch, startDate, endDate);
        return Ok(contracts);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Contract>> GetContract(int id)
    {
        var contract = await _contracts.GetByIdAsync(id);
        return contract == null ? NotFound() : Ok(contract);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Contract), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Contract>> CreateContract(ContractRequest request)
    {
        var contract = new Contract
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            ServiceLevel = request.ServiceLevel,
            AgreementFilePath = request.AgreementFilePath,
            ClientId = request.ClientId
        };

        var created = await _contracts.CreateAsync(contract);
        return CreatedAtAction(nameof(GetContract), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContract(int id, ContractRequest request)
    {
        var contract = new Contract
        {
            Id = id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            ServiceLevel = request.ServiceLevel,
            AgreementFilePath = request.AgreementFilePath,
            ClientId = request.ClientId
        };

        return await _contracts.UpdateAsync(contract) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContractStatus(
        int id,
        ContractStatusUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest("Status is required.");
        }

        return await _contracts.UpdateStatusAsync(id, request.Status)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContract(int id)
    {
        return await _contracts.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
