using DBlockchain.Infrastructure.Command.Contracts;

namespace DBlockchain.Infrastructure.Network.Fabrics.Contracts
{
    public interface IRequestFabric
    {
        void MakeRequest(string commandName, string[] args, IGlobalCommand command, Commands.Attributes.Command attribute);
    }
}
