using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Wallet;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("create-wallet", "{0}", CommandType.Local, "Creates wallet if not exist")]
    public class WalletCreation : ILocalCommand
    {
        private Blockchain blockchain;

        public WalletCreation()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public void Run(string[] args)
        {
            Console.WriteLine("Wallet creation...");

            WalletProvider walletProvider = new WalletProvider();

            if (!walletProvider.HasWallet())
            {
                walletProvider.GenerateWallet();
            }
            else
            {
                Console.WriteLine("Already has wallet...");
                Console.WriteLine(walletProvider.Address);
            }
        }
    }
}
