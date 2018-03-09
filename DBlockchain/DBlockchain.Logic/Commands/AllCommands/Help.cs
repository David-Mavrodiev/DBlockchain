using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Command.Helpers;
using DBlockchain.Infrastructure.Commands.Attributes;
using System;
using System.Linq;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("help", "{0}", CommandType.Local)]
    public class Help : ILocalCommand
    {
        public void Run(string[] args)
        {
            var commandsInfo = CommandsReflector.GetAllCommandsInfo().OrderBy(x => x.Name).ToList();

            foreach (var commandInfo in commandsInfo)
            {
                Console.WriteLine();
                Console.WriteLine($"Command: {commandInfo.Name}");

                var syntax = commandInfo.Template;
                syntax = syntax.Replace("{0}", commandInfo.Name);
                syntax.Trim();

                Console.WriteLine($"> syntax: {syntax}");
                Console.WriteLine($"> description: {commandInfo.Description}");
            }
        }
    }
}
