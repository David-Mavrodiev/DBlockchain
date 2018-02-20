using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("show-threads", "{0}", CommandType.Local)]
    public class ShowThreads : ILocalCommand
    {
        public void Run(string[] args)
        {
            if (CommandFabric.ThreadsInfo.Count == 0)
            {
                Console.WriteLine("No threads...");
                return;
            }

            Console.WriteLine("-------------------------");
            int count = 1;

            foreach (var info in CommandFabric.ThreadsInfo)
            {
                Console.WriteLine($"{count}.) {info.Key} => {info.Value}");
                count++;
            }

            Console.WriteLine("-------------------------");
        }
    }
}
