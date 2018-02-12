using DBlockchain.Infrastructure.Network;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using DBlockchain.Logic.Commands.Contracts;

namespace DBlockchain.Logic.Commands.Fabrics
{
    public class CommandFabric : ICommandFabric
    {
        private readonly IRequestFabric requestFabric;
        private readonly AsyncListener listener;

        public CommandFabric(IRequestFabric requestFabric, AsyncListener listener)
        {
            this.requestFabric = requestFabric;
            this.listener = listener;
            listener.StartListening();
        }

        public void RunCommand(string input)
        {
            var name = input.Split(' ')[0];

            requestFabric.MakeRequest(name, input);
        }
    }
}
