using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Network.Fabrics.Contracts;
using System;
using System.Linq;
using System.Reflection;

namespace DBlockchain.Infrastructure.Network.Fabrics
{
    public class ResponseFabric : IResponseFabric
    {
        public void ReceiveResponse(SocketDataBody data)
        {
            var commandName = data.CommandName;
            
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

            foreach (var type in exportedTypes)
            {
                var commandAttribute = type.GetCustomAttributes(typeof(Commands.Attributes.Command), true)
                    .Cast<Commands.Attributes.Command>().FirstOrDefault();

                if (commandAttribute.name == commandName)
                {
                    command = (ICommand)Activator.CreateInstance(type);

                    break;
                }
            }

            command.Receive(data);
        }
    }
}
