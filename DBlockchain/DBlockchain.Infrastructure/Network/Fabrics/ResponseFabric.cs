using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;

namespace DBlockchain.Infrastructure.Network.Fabrics
{
    public class ResponseFabric : IResponseFabric
    {
        public void ReceiveResponse(SocketDataBody data)
        {
            var commandName = data.CommandName;

            var command = CommandsReflector.GetCommand(commandName).Item1;

            command.Receive(data);
        }
    }
}
