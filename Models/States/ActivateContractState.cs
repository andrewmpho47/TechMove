namespace TechMove.Models.States
{
    public class ActiveContractState : IContractState
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