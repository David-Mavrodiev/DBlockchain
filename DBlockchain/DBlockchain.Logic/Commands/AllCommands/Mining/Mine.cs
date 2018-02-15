using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Contracts;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Utils;
using Ninject;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DBlockchain.Logic.Commands.AllCommands.Mining
{
    [Command("mine", "{0}", CommandType.Local)]
    public class Mine : ILocalCommand
    {
        private Blockchain blockchain;

        public Mine()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public void Run(string[] args)
        {
            Console.WriteLine("Start mining...");

            Thread thread = new Thread(MineAsync);
            thread.Start();
        }

        public void NextBlockMined(string hash, int nonce)
        {
            Console.WriteLine();
            Console.WriteLine("+++++++++++++++++++++++++");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Found hash: {hash}...");
            Console.ResetColor();
            Console.WriteLine("+++++++++++++++++++++++++");

            CommandFabric.RunDynamic($"send-mining-result -nonce {nonce}");
        }

        public void MineAsync()
        {
            int nonce = 0;
            string hash;

            while (true)
            {
                var lastBlockHash = this.blockchain.LastBlock.BlockHash;
                hash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256($"{lastBlockHash}{nonce}"));

                if (hash.ToCharArray().Take(this.blockchain.Difficulty).All(s => s == '0'))
                {
                    
                    break;
                }

                nonce++;
            }

            NextBlockMined(hash, nonce);

            Thread thread = new Thread(MineAsync);
            thread.Start();
        }
    }
}
