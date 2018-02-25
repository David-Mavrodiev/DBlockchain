using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Commands.Fabrics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DBlockchain.Logic.Commands.AllCommands.Store
{
    [Command("start-storage", "{0}", CommandType.Local)]
    public class StartStorage : ILocalCommand
    {
        private DFileStorage fileStorrage;

        public StartStorage()
        {
            List<Tuple<string, int>> peers = new List<Tuple<string, int>>();

            foreach (var peer in CommandFabric.Blockchain.Peers)
            {
                peers.Add(new Tuple<string, int>(peer.Address, peer.Port));
            }

            fileStorrage = new DFileStorage(peers);
        }

        public void Run(string[] args)
        {
            Console.WriteLine("Start decentralized storage...");

            Thread thread = new Thread(fileStorrage.StartFileListener);
            thread.Start();
        }
    }
}
