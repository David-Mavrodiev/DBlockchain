using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Wallet;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("account-info", "{0}", CommandType.Local)]
    public class GetAccountInfo : ILocalCommand
    {
        private WalletProvider walletProvider;

        public GetAccountInfo()
        {
            this.walletProvider = new WalletProvider();
        }

        public void Run(string[] args)
        {
            if (this.walletProvider.HasWallet())
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Address: {this.walletProvider.Address}");
                Console.WriteLine($"Public key: {CryptographyUtilities.BytesToHex(this.walletProvider.PublicKey.GetEncoded())}");
                Console.WriteLine("-------------------------");
            }
            else
            {
                Console.WriteLine("None wallet found...");
            }
        }
    }
}
