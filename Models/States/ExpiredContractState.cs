namespace TechMove.Models.States
{
    public class ExpiredContractState : IContractState
    {
        public bool CanCreateServiceRequest()
        {
            return false;
        }

        public string GetBlockedMessage()
        {
            return "Service Request cannot be created for Expired contracts.";
        }
    }
}