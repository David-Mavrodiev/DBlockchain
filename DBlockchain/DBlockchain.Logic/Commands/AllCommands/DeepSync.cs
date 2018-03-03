using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using Newtonsoft.Json;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("deep-sync", "{0} -ip {1} -p {2}", CommandType.Global)]
    public class DeepSync : IGlobalCommand
    {
        private Blockchain blockchain;

        public DeepSync()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public string Aggregate(SocketDataBody data)
        {
            var blockHashes = JsonConvert.DeserializeObject<string[]>(data.Body).ToList();
            blockHashes.Reverse();

            foreach (var blockHash in blockHashes)
            {
                var block = this.blockchain.Blocks.FirstOrDefault(b => b.BlockHash == blockHash);

                if (block != null)
                {
                    var lastValidBlockIndex = block.Index;

                    var blocks = this.blockchain.Blocks.GetRange(lastValidBlockIndex,
                        (this.blockchain.Blocks.Count - lastValidBlockIndex)).ToArray();

                    return JsonConvert.SerializeObject(blocks);
                }
            }

            return string.Empty;
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
                }
            }
        }

        public string Send(string[] args)
        {
            var blockHashes = this.blockchain.Blocks.OrderBy(b => b.Index).Select(b => b.BlockHash).ToArray();

            if (blockHashes.Length > 50)
            {
                blockHashes = blockHashes.ToList().GetRange((blockHashes.Length - 50), 50).ToArray();
            }

            return JsonConvert.SerializeObject(blockHashes);
        }

        public bool ValidateInput(string[] args)
        {
            return true;
        }
    }
}
