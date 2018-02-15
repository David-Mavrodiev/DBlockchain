using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using System;
using System.Net;

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

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string ip = Dns.GetHostByName(hostName).AddressList[0].ToString();

            foreach (var target in targets)
            {
                var socket = client.StartSocket(target.Item1, target.Item2);

                var data = new SocketDataBody()
                {
                    CommandName = commandName,
                    Body = body,
                    Type = SocketDataType.Send,
                    NodesPair = new Tuple<string, string>($"{ip}:{AsyncListener.Port}", $"{target.Item1.ToString()}:{target.Item2}")
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
