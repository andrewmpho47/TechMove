namespace TechMove.Models.States
{
    public class PendingContractState : IContractState
    {
        public bool CanCreateServiceRequest()
        {
            return true;
        }

        public string GetBlockedMessage()
        {
            return string.Empty;
        }
    }
}