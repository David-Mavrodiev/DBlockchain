using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Wallet;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("account-info", "{0}", CommandType.Local, "Shows your address, public key and transaction history")]
    public class GetAccountInfo : ILocalCommand
    {
        private WalletProvider walletProvider;
        private Blockchain blockchain;

        public GetAccountInfo()
        {
            this.blockchain = CommandFabric.Blockchain;
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
                Console.WriteLine("Transaction history");
                PrintTransactionHistory(this.walletProvider.Address);
            }
            else
            {
                Console.WriteLine("None wallet found...");
            }
        }

        public void PrintTransactionHistory(string address)
        {
            foreach (var block in this.blockchain.Blocks)
            {
                foreach (var transaction in block.Transactions)
                {
                    if (transaction.From != address && transaction.To != address)
                    {
                        continue;
                    }

                    if (transaction.From == address)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("OUT");
                        Console.ResetColor();
                    }

                    if (transaction.To == address)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("IN");
                        Console.ResetColor();
                    }

                    Console.WriteLine($" {transaction.From} -> {transaction.To} ({transaction.Value})");
                }
            }
        }
    }
}
