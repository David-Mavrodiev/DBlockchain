using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DBlockchain.Infrastructure.Network.Fabrics
{
    public class RequestFabric : IRequestFabric
    {
        private AsyncClient client;

        public RequestFabric(AsyncClient client)
        {
            this.client = client;
        }

        public void MakeRequest(string commandName, string input)
        {
            var tuple = CommandsReflector.GetCommand(commandName);
            var command = tuple.Item1;
            var template = tuple.Item2.template;

            var args = ReverseStringFormat(template, input).ToArray();
            var body = command.Send(args);

            var targets = command.GetTargets(args);

            foreach (var target in targets)
            {
                var socket = client.StartSocket(target.Item1, target.Item2);

                var data = new SocketDataBody()
                {
                    CommandName = commandName,
                    Body = body
                };

                client.Send(socket, data);
                AsyncClient.sendDone.WaitOne();

                client.Receive(socket);
                AsyncClient.receiveDone.WaitOne();
            }
        }

        private static List<string> ReverseStringFormat(string template, string str)
        {
            string pattern = "^" + Regex.Replace(template, @"\{[0-9]+\}", "(.*?)") + "$";

            Regex r = new Regex(pattern);
            Match m = r.Match(str);

            List<string> ret = new List<string>();

            for (int i = 1; i < m.Groups.Count; i++)
            {
                ret.Add(m.Groups[i].Value);
            }

            return ret;
        }
    }
}
