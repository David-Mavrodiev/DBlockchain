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
            var lastBlockInfo = JsonConvert.DeserializeObject<Tuple<int, string>>(data.Body);
            var lastBlockIndex = lastBlockInfo.Item1;
            var lastBlockHash = lastBlockInfo.Item2;

            if (lastBlockIndex < this.blockchain.LastBlock.Index)
            {
                if (this.blockchain.Blocks[lastBlockIndex].BlockHash == lastBlockHash)
                {
                    var newBlocks = this.blockchain.Blocks.Skip(lastBlockIndex).ToArray();

                    return JsonConvert.SerializeObject(newBlocks);
                }
                else
                {
                    int skip = 0;

                    foreach (var block in this.blockchain.Blocks)
                    {
                        if (block.BlockHash == lastBlockHash)
                        {
                            skip = block.Index;
                            break;
                        }
                    }

                    var newBlocks = this.blockchain.Blocks.Skip(skip).ToArray();

                    return JsonConvert.SerializeObject(newBlocks);
                }
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
                    Console.WriteLine("Adding new blocks...");
                    var newBlocks = JsonConvert.DeserializeObject<Block[]>(data.Body).ToList();
                    newBlocks.Reverse();

                    foreach (var block in newBlocks)
                    {
                        this.blockchain.AddBlock(block);
                    }
                }
                else
                {
                    Console.WriteLine("Already is up-to-date...");
                }
            }
        }

        public string Send(string[] args)
        {
            var lastBlockInfo = new Tuple<int, string>(this.blockchain.LastBlock.Index, this.blockchain.LastBlock.BlockHash);

            return JsonConvert.SerializeObject(lastBlockInfo);
        }

        public bool ValidateInput(string[] args)
        {
            return true;
        }
    }
}
