using System.Net;
using System.Net.Http.Json;
using TechMove.Models;

namespace TechMove.Services;

public class TechMoveApiClient
{
    private readonly HttpClient _httpClient;

    public TechMoveApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        return await _httpClient.GetFromJsonAsync<DashboardViewModel>(
            "api/dashboard")
            ?? new DashboardViewModel();
    }

    public async Task<List<Client>> GetClientsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Client>>("api/clients")
            ?? new List<Client>();
    }

    public async Task<Client?> GetClientAsync(int id)
    {
        return await GetOrNullAsync<Client>($"api/clients/{id}");
    }

    public async Task<ApiResult> CreateClientAsync(Client client)
    {
        var response = await _httpClient.PostAsJsonAsync("api/clients", client);
        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> UpdateClientAsync(Client client)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/clients/{client.Id}",
            client);

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> DeleteClientAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/clients/{id}");
        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<List<Contract>> GetContractsAsync(
        string? statusSearch = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = BuildContractQuery(statusSearch, startDate, endDate);

        return await _httpClient.GetFromJsonAsync<List<Contract>>(
            $"api/contracts{query}")
            ?? new List<Contract>();
    }

    public async Task<Contract?> GetContractAsync(int id)
    {
        return await GetOrNullAsync<Contract>($"api/contracts/{id}");
    }

    public async Task<ApiResult> CreateContractAsync(Contract contract)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/contracts",
            contract);

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> UpdateContractAsync(Contract contract)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/contracts/{contract.Id}",
            contract);

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> UpdateContractStatusAsync(
        int id,
        string status)
    {
        var response = await _httpClient.PatchAsJsonAsync(
            $"api/contracts/{id}/status",
            new { status });

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> DeleteContractAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ServiceRequest>>(
            "api/servicerequests")
            ?? new List<ServiceRequest>();
    }

    public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
    {
        return await GetOrNullAsync<ServiceRequest>(
            $"api/servicerequests/{id}");
    }

    public async Task<ApiResult> CreateServiceRequestAsync(
        ServiceRequest serviceRequest)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/servicerequests",
            serviceRequest);

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> UpdateServiceRequestAsync(
        ServiceRequest serviceRequest)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/servicerequests/{serviceRequest.Id}",
            serviceRequest);

        return await ApiResult.FromResponseAsync(response);
    }

    public async Task<ApiResult> DeleteServiceRequestAsync(int id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/servicerequests/{id}");

        return await ApiResult.FromResponseAsync(response);
    }

    private async Task<T?> GetOrNullAsync<T>(string path)
    {
        var response = await _httpClient.GetAsync(path);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    private static string BuildContractQuery(
        string? statusSearch,
        DateTime? startDate,
        DateTime? endDate)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(statusSearch))
        {
            query.Add($"statusSearch={Uri.EscapeDataString(statusSearch)}");
        }

        if (startDate.HasValue)
        {
            query.Add($"startDate={startDate:yyyy-MM-dd}");
        }

        if (endDate.HasValue)
        {
            query.Add($"endDate={endDate:yyyy-MM-dd}");
        }

        return query.Count == 0 ? string.Empty : "?" + string.Join("&", query);
    }
}

public class ApiResult
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static async Task<ApiResult> FromResponseAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return new ApiResult { Success = true };
        }

        var message = await response.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = false,
            ErrorMessage = string.IsNullOrWhiteSpace(message)
                ? $"API request failed with status {(int)response.StatusCode}."
                : message
        };
    }
}
