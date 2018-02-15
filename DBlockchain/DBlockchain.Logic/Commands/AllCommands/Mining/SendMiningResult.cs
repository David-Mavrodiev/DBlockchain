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

        public string Aggregate(SocketDataBody data)
        {
            return this.blockchain.LastBlock.Index.ToString();
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
            Console.WriteLine($"{data.NodesPair.Item1} -> {data.NodesPair.Item2}");
            if (data.Type == SocketDataType.Send)
            {
                var block = JsonConvert.DeserializeObject<Block>(data.Body);

                if (block.Index == this.blockchain.LastBlock.Index + 1 &&
                    block.PreviousBlockHash == this.blockchain.LastBlock.BlockHash)
                {
                    string lastBlockHash = this.blockchain.LastBlock.BlockHash;
                    int nonce = block.Nonce;
                    string winnerHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256($"{lastBlockHash}{nonce}"));

                    if (!winnerHash.ToCharArray().Take(this.blockchain.Difficulty).All(s => s == '0'))
                    {
                        Console.WriteLine("Incorrect hash...");
                        return;
                    }

                    this.blockchain.AddBlock(block);
                }
                else
                {
                    Console.WriteLine("This node may not synced with 2 or more blocks...");
                    Console.WriteLine("Checking...");

                    var ip = data.NodesPair.Item1.Split(':')[0];
                    var port = int.Parse(data.NodesPair.Item1.Split(':')[1]);

                    CommandFabric.RunDynamic($"sync -ip {ip} -p {port}");
                }
            }
            else if(data.Type == SocketDataType.Receive)
            {
                int blockIndex = int.Parse(data.Body);
                if (blockIndex > this.blockchain.LastBlock.Index)
                {
                    Console.WriteLine("This node may not synced with 1 or more blocks...");
                    Console.WriteLine("Checking...");

                    var ip = data.NodesPair.Item2.Split(':')[0];
                    var port = int.Parse(data.NodesPair.Item2.Split(':')[1]);

                    CommandFabric.RunDynamic($"sync -ip {ip} -p {port}");
                }
            }
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
