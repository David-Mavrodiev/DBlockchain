using DBlockchain.Infrastructure.Command.Contracts;
using System;
using System.Linq;
using System.Reflection;

namespace DBlockchain.Infrastructure.Command.Helpers
{
    public static class CommandsReflector
    {
        public static readonly string module = "DBlockchain.Logic";
            
        public static Tuple<ICommand, Commands.Attributes.Command> GetCommand(string commandName)
        {
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
            Commands.Attributes.Command attribute = null;

            foreach (var type in exportedTypes)
            {
                var commandAttribute = type.GetCustomAttributes(typeof(Commands.Attributes.Command), true)
                    .Cast<Commands.Attributes.Command>().FirstOrDefault();

                if (commandAttribute.name == commandName)
                {
                    command = (ICommand)Activator.CreateInstance(type);
                    attribute = commandAttribute;
                    break;
                }
            }

            return new Tuple<ICommand, Commands.Attributes.Command>(command, attribute);
        }
    }
}
