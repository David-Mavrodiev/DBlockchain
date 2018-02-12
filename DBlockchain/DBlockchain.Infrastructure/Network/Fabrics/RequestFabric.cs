using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var module = "DBlockchain.Logic";
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == module);
            if (assembly == null)
            {
                try
                {
                    assembly = Assembly.Load(module);
                }
                catch
                {

                }
            }

            var exportedTypes = assembly.ExportedTypes
                .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t) &&
                t.GetCustomAttributes(typeof(Commands.Attributes.Command)).Count() > 0).ToList();

            ICommand command = null;
            string template = null;

            foreach (var type in exportedTypes)
            {
                var commandAttribute = type.GetCustomAttributes(typeof(Commands.Attributes.Command), true)
                    .Cast<Commands.Attributes.Command>().FirstOrDefault();

                if (commandAttribute.name == commandName)
                {
                    command = (ICommand)Activator.CreateInstance(type);
                    template = commandAttribute.template;
                    break;
                }
            }

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
