using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Models;
using System;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("peers", "{0}", CommandType.Local)]
    public class GetPeers : ILocalCommand
    {
        private Blockchain blockchain;

        public GetPeers()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public void Run(string[] args)
        {
            Console.WriteLine("+++++++++++++++++++++++++");
            var peers = this.blockchain.Peers;

            foreach (var peer in peers)
            {
                Console.WriteLine($"{peer.Address}:{peer.Port}");
            }
            Console.WriteLine("+++++++++++++++++++++++++");
        }
    }
}
