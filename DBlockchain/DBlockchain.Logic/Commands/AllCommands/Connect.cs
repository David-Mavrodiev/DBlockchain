using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("connect", "{0} -ip {1} -p {2}")]
    public class Connect : ICommand
    {
        private Blockchain blockchain;

        public Connect()
        {
            this.blockchain = new Blockchain();
        }

        public string Send(string[] args)
        {
            Console.WriteLine("Try to connect...");
            return null;
        }

        public void Receive(SocketDataBody data)
        {
            Console.WriteLine("Receive connection...");
            Console.WriteLine($"Command: {data.CommandName}, Body: {data.Body}");
        }

        public List<Tuple<IPAddress, int>> GetTargets(string[] args)
        {
            var targets = new List<Tuple<IPAddress, int>>();

            targets.Add(new Tuple<IPAddress, int>(IPAddress.Parse(args[1]), int.Parse(args[2])));

            return targets;
        }

        public string Aggregate()
        {
            return "response body";
        }
    }
}
