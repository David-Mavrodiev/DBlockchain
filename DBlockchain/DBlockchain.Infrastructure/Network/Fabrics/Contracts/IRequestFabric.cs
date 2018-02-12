namespace DBlockchain.Infrastructure.Network.Fabrics.Contracts
{
    public interface IRequestFabric
    {
        void MakeRequest(string commandName, string input);
    }
}
