namespace TechMove.Models.States
{
    public class OnHoldContractState : IContractState
    {
        public bool CanCreateServiceRequest()
        {
            return false;
        }

        public string GetBlockedMessage()
        {
            return "Service Request cannot be created for On Hold contracts.";
        }
    }
}