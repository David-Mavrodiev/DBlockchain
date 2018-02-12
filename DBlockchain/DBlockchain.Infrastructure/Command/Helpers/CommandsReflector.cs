using DBlockchain.Infrastructure.Command.Contracts;
using System;
using System.Linq;
using System.Reflection;

namespace DBlockchain.Infrastructure.Command.Helpers
{
    public static class CommandsReflector
    {
        public static readonly string module = "DBlockchain.Logic";
            
        public static Tuple<IGlobalCommand, Commands.Attributes.Command> GetGlobalCommand(string commandName)
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
                .Where(t => t.IsClass && typeof(IGlobalCommand).IsAssignableFrom(t) &&
                t.GetCustomAttributes(typeof(Commands.Attributes.Command)).Count() > 0).ToList();

            IGlobalCommand command = null;
            Commands.Attributes.Command attribute = null;

            foreach (var type in exportedTypes)
            {
                var commandAttribute = type.GetCustomAttributes(typeof(Commands.Attributes.Command), true)
                    .Cast<Commands.Attributes.Command>().FirstOrDefault();

                if (commandAttribute.Name == commandName)
                {
                    command = (IGlobalCommand)Activator.CreateInstance(type);
                    attribute = commandAttribute;
                    break;
                }
            }

            return new Tuple<IGlobalCommand, Commands.Attributes.Command>(command, attribute);
        }

        public static Tuple<ILocalCommand, Commands.Attributes.Command> GetLocalCommand(string commandName)
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
                .Where(t => t.IsClass && typeof(ILocalCommand).IsAssignableFrom(t) &&
                t.GetCustomAttributes(typeof(Commands.Attributes.Command)).Count() > 0).ToList();

            ILocalCommand command = null;
            Commands.Attributes.Command attribute = null;

            foreach (var type in exportedTypes)
            {
                var commandAttribute = type.GetCustomAttributes(typeof(Commands.Attributes.Command), true)
                    .Cast<Commands.Attributes.Command>().FirstOrDefault();

                if (commandAttribute.Name == commandName)
                {
                    command = (ILocalCommand)Activator.CreateInstance(type);
                    attribute = commandAttribute;
                    break;
                }
            }

            return new Tuple<ILocalCommand, Commands.Attributes.Command>(command, attribute);
        }
    }
}
