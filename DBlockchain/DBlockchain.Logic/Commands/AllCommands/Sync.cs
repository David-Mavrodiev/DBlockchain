using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Commands.Fabrics;
using Newtonsoft.Json;
using System.Linq;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("sync", "{0} -ip {1} -p {2}", CommandType.Global)]
    public class Sync : IGlobalCommand
    {
        private Blockchain blockchain;

        public Sync()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public string Aggregate(SocketDataBody data)
        {
            Console.WriteLine("Sync aggregate");
            var lastBlockInfo = JsonConvert.DeserializeObject<Tuple<int, string>>(data.Body);
            var lastBlockIndex = lastBlockInfo.Item1;
            var lastBlockHash = lastBlockInfo.Item2;

            if (lastBlockIndex <= this.blockchain.LastBlock.Index)
            {
                return JsonConvert.SerializeObject(this.blockchain.Blocks.Where(b => b.Index != 0).ToArray());
            }
            else
            {
                return string.Empty;
            }
        }

        public List<Tuple<IPAddress, int>> GetTargets(string[] args)
        {
            var targets = new List<Tuple<IPAddress, int>>();

            targets.Add(new Tuple<IPAddress, int>(IPAddress.Parse(args[1]), int.Parse(args[2])));

            return targets;
        }

        public void Receive(SocketDataBody data)
        {
            if (data.Type == SocketDataType.Receive)
            {
                if (data.Body != string.Empty)
                {
                    var newBlocks = JsonConvert.DeserializeObject<Block[]>(data.Body).ToList();
                    
                    this.blockchain.RemoveBlockInterval(newBlocks.First().Index, newBlocks.Last().Index);

                    foreach (var block in newBlocks)
                    {
                        this.blockchain.AddBlock(block);
                    }

                    Console.WriteLine("End syncing...");
                }
                else
                {
                    Console.WriteLine("Already is up-to-date...");
                }
            }
        }

        public string Send(string[] args)
        {
            Console.WriteLine("Start syncing...");
            var lastBlockInfo = new Tuple<int, string>(this.blockchain.LastBlock.Index, this.blockchain.LastBlock.BlockHash);

            return JsonConvert.SerializeObject(lastBlockInfo);
        }

        public bool ValidateInput(string[] args)
        {
            return true;
        }
    }
}
