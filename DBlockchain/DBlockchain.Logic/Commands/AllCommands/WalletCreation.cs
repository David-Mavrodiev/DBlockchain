using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("create-wallet", "{0}", CommandType.Local)]
    public class WalletCreation : ILocalCommand
    {
        public void Run(string[] args)
        {
            Console.WriteLine("Wallet creation...");
        }
    }
}
