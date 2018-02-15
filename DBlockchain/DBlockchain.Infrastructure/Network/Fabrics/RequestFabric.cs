using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;

namespace DBlockchain.Infrastructure.Network.Fabrics
{
    public class RequestFabric : IRequestFabric
    {
        private AsyncClient client;

        public RequestFabric(AsyncClient client)
        {
            this.client = client;
        }

        public void MakeRequest(string commandName, string[] args, IGlobalCommand command, Commands.Attributes.Command attribute)
        {
            var template = attribute.Template;

            var body = command.Send(args);

            var targets = command.GetTargets(args);

            foreach (var target in targets)
            {
                var socket = client.StartSocket(target.Item1, target.Item2);

                var data = new SocketDataBody()
                {
                    CommandName = commandName,
                    Body = body,
                    Type = SocketDataType.Send
                };

                client.Send(socket, data);
                AsyncClient.sendDone.WaitOne();

                client.Receive(socket);
                AsyncClient.receiveDone.WaitOne();

                //client.StopSocket(socket);
            }
        }
    }
}
