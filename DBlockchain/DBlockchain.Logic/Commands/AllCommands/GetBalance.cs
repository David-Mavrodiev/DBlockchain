using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("balance", "{0} -address {1} -confirms {2}", CommandType.Local)]
    public class GetBalance : ILocalCommand
    {
        public readonly Blockchain blockchain;

        public GetBalance()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public void Run(string[] args)
        {
            var address = args[1];
            var confirmations = int.Parse(args[2]);
      
            var balances = this.blockchain.GetBalances(confirmations);
            if (balances.ContainsKey(address))
            {
                Console.WriteLine($"{balances[address]} coins...");
            }
            else
            {
                Console.WriteLine("This address is not in register...");
            }
        }
    }
}
