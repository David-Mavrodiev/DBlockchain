using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Utils;
using System.Linq;
using DBlockchain.Logic.Wallet;
using Newtonsoft.Json;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("send-mining-result", "{0} -nonce {1}", CommandType.Global)]
    public class SendMiningResult : IGlobalCommand
    {
        private Blockchain blockchain;
        private WalletProvider walletProvider;

        public SendMiningResult()
        {
            this.walletProvider = new WalletProvider();
            this.blockchain = CommandFabric.Blockchain;
        }

        public string Aggregate()
        {
            return "Aggregate";
        }

        public List<Tuple<IPAddress, int>> GetTargets(string[] args)
        {
            var peers = new List<Tuple<IPAddress, int>>();

            foreach (var peer in this.blockchain.Peers)
            {
                peers.Add(new Tuple<IPAddress, int>(IPAddress.Parse(peer.Address), peer.Port));
            }

            return peers;
        }

        public void Receive(SocketDataBody data)
        {
            Console.WriteLine("Receive");
        }

        public string Send(string[] args)
        {
            int nonce = int.Parse(args[1]);

            var block = this.blockchain.AddBlock(nonce, walletProvider);

            return JsonConvert.SerializeObject(block);
        }

        public bool ValidateInput(string[] args)
        {
            string lastBlockHash = this.blockchain.LastBlock.BlockHash;
            int nonce = int.Parse(args[1]);
            string winnerHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256($"{lastBlockHash}{nonce}"));

            if (!winnerHash.ToCharArray().Take(this.blockchain.Difficulty).All(s => s == '0'))
            {
                Console.WriteLine("Incorrect hash...");
                return false;
            }

            return true;
        }
    }
}
