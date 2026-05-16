using TechMove.Models.States;

namespace TechMove.Services
{
    public class ContractStateService
    {
        public IContractState GetState(string? status)
        {
            return status?.Trim().ToLower() switch
            {
                "active" => new ActiveContractState(),
                "pending" => new PendingContractState(),
                "expired" => new ExpiredContractState(),
                "on hold" => new OnHoldContractState(),
                _ => new PendingContractState()
            };
        }
    }
}