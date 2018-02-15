using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Logic.Commands.Fabrics;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("connect", "{0} -ip {1} -p {2}", CommandType.Global)]
    public class Connect : IGlobalCommand
    {
        private Blockchain blockchain;

        public Connect()
        {
            this.blockchain = CommandFabric.Blockchain;
        }

        public string Send(string[] args)
        {
            Console.WriteLine("Try to connect...");
            return null;
        }

        public void Receive(SocketDataBody data)
        {
            if (data.Type == SocketDataType.Receive && data.Body != null)
            {
                var ip = data.Body.Split(':')[0];
                var port = int.Parse(data.Body.Split(':')[1]);

                var isAdded = this.blockchain.RegisterNode(ip, port);

                if (isAdded)
                {
                    Console.WriteLine($"{ip}:{port}");
                    Console.WriteLine("Peer added successfully...");
                }
                else
                {
                    Console.WriteLine("Cannot add peer...");
                }
            }

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
            var port = AsyncListener.Port;
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string ip = Dns.GetHostByName(hostName).AddressList[0].ToString();

            return $"{ip}:{port}";
        }
    }
}
