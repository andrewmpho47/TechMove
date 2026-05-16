namespace TechMove.Models.States
{
    public interface IContractState
    {
        bool CanCreateServiceRequest();

        string GetBlockedMessage();
    }
}