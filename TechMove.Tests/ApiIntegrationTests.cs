using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TechMove.Api;
using TechMove.Data;
using TechMove.Models;

namespace TechMove.Tests
{
    public class ApiIntegrationTests :
        IClassFixture<WebApplicationFactory<ApiAssemblyMarker>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(
            WebApplicationFactory<ApiAssemblyMarker> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var databaseName = $"TechMoveTests-{Guid.NewGuid()}";

                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.RemoveAll<
                        IDbContextOptionsConfiguration<ApplicationDbContext>>();

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(databaseName));

                    using var scope = services.BuildServiceProvider()
                        .CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationDbContext>();

                    context.Database.EnsureCreated();
                    Seed(context);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetContracts_ReturnsOkAndJson()
        {
            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts =
                await response.Content.ReadFromJsonAsync<List<Contract>>();

            Assert.NotNull(contracts);
            Assert.NotEmpty(contracts);
        }

        [Fact]
        public async Task PostContract_CreatesContractWithCreatedStatus()
        {
            var client = await CreateClientAsync();

            var contract = new Contract
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6),
                Status = "Pending",
                ServiceLevel = "Standard",
                ClientId = client.Id
            };

            var response =
                await _client.PostAsJsonAsync("/api/contracts", contract);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created =
                await response.Content.ReadFromJsonAsync<Contract>();

            Assert.NotNull(created);
            Assert.True(created.Id > 0);
        }

        [Fact]
        public async Task PatchContractStatus_UpdatesStatus()
        {
            var client = await CreateClientAsync();
            var contract = await CreateContractAsync(client.Id);

            var response = await _client.PatchAsJsonAsync(
                $"/api/contracts/{contract.Id}/status",
                new { status = "Approved" });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updated = await _client.GetFromJsonAsync<Contract>(
                $"/api/contracts/{contract.Id}");

            Assert.NotNull(updated);
            Assert.Equal("Approved", updated.Status);
        }

        private async Task<Client> CreateClientAsync()
        {
            var client = new Client
            {
                Name = "Integration Test Client",
                ContactDetails = "test@example.com",
                Region = "Gauteng"
            };

            var response = await _client.PostAsJsonAsync("/api/clients", client);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<Client>())!;
        }

        private async Task<Contract> CreateContractAsync(int clientId)
        {
            var contract = new Contract
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                Status = "Pending",
                ServiceLevel = "Premium",
                ClientId = clientId
            };

            var response =
                await _client.PostAsJsonAsync("/api/contracts", contract);

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<Contract>())!;
        }

        private static void Seed(ApplicationDbContext context)
        {
            var client = new Client
            {
                Name = "Seed Client",
                ContactDetails = "seed@example.com",
                Region = "Western Cape"
            };

            context.Clients.Add(client);
            context.SaveChanges();

            context.Contracts.Add(new Contract
            {
                StartDate = DateTime.Today.AddDays(-7),
                EndDate = DateTime.Today.AddMonths(12),
                Status = "Active",
                ServiceLevel = "Premium",
                ClientId = client.Id
            });

            context.SaveChanges();
        }
    }
}
