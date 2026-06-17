using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TechMove.Api.Dtos;
using TechMove.Api.Repositories;
using TechMove.Models;
using TechMove.Services;

namespace TechMove.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly IServiceRequestRepository _serviceRequests;
    private readonly IContractRepository _contracts;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ContractStateService _contractStateService;

    public ServiceRequestsController(
        IServiceRequestRepository serviceRequests,
        IContractRepository contracts,
        IHttpClientFactory httpClientFactory,
        ContractStateService contractStateService)
    {
        _serviceRequests = serviceRequests;
        _contracts = contracts;
        _httpClientFactory = httpClientFactory;
        _contractStateService = contractStateService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceRequest>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceRequest>>> GetServiceRequests()
    {
        return Ok(await _serviceRequests.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ServiceRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceRequest>> GetServiceRequest(int id)
    {
        var serviceRequest = await _serviceRequests.GetByIdAsync(id);
        return serviceRequest == null ? NotFound() : Ok(serviceRequest);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServiceRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceRequest>> CreateServiceRequest(
        ServiceRequestRequest request)
    {
        var contract = await _contracts.GetByIdAsync(request.ContractId);

        if (contract == null)
        {
            return BadRequest("The selected contract does not exist.");
        }

        var contractState = _contractStateService.GetState(contract.Status);

        if (!contractState.CanCreateServiceRequest())
        {
            return BadRequest(contractState.GetBlockedMessage());
        }

        var serviceRequest = new ServiceRequest
        {
            Description = request.Description,
            CostUSD = request.CostUSD,
            Status = request.Status,
            ContractId = request.ContractId
        };

        serviceRequest.CostZAR = await ConvertUsdToZar(serviceRequest.CostUSD);

        var created = await _serviceRequests.CreateAsync(serviceRequest);

        return CreatedAtAction(
            nameof(GetServiceRequest),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateServiceRequest(
        int id,
        ServiceRequestRequest request)
    {
        var serviceRequest = new ServiceRequest
        {
            Id = id,
            Description = request.Description,
            CostUSD = request.CostUSD,
            Status = request.Status,
            ContractId = request.ContractId
        };

        serviceRequest.CostZAR = await ConvertUsdToZar(serviceRequest.CostUSD);

        return await _serviceRequests.UpdateAsync(serviceRequest)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServiceRequest(int id)
    {
        return await _serviceRequests.DeleteAsync(id) ? NoContent() : NotFound();
    }

    private async Task<decimal> ConvertUsdToZar(decimal usdAmount)
    {
        try
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

            return usdAmount * exchangeRateResponse.Rates["ZAR"];
        }
        catch
        {
            return usdAmount * 18;
        }
    }
}
