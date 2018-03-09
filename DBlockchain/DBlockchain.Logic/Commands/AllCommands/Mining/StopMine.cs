using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using System.Linq;

namespace DBlockchain.Logic.Commands.AllCommands.Mining
{
    [Command("stop-mine", "{0}", CommandType.Local, "Stop mining")]
    public class StopMine : ILocalCommand
    {
        public void Run(string[] args)
        {
            if (Mine.IsMining)
            {
                System.Console.WriteLine("Stop mining...");
                Mine.StopThread = true;
                Mine.IsMining = false;

                var miningProcesses = CommandFabric.ThreadsInfo.Where(i => i.Key == "Mining").ToArray();

                foreach (var process in miningProcesses)
                {
                    CommandFabric.ThreadsInfo.Remove(process);
                }
            }
            else
            {
                System.Console.WriteLine("Not mining...");
            }
        }
    }
}
