﻿using System;
using System.Collections.Generic;
using System.Net;
using DBlockchain.Infrastructure.Command.Contracts;
using DBlockchain.Infrastructure.Command.Enums;
using DBlockchain.Infrastructure.Commands.Attributes;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Commands.Fabrics;
using DBlockchain.Logic.Wallet;
using DBlockchain.Logic.Utils;
using Newtonsoft.Json;

namespace DBlockchain.Logic.Commands.AllCommands
{
    [Command("send", "{0} -to {1} -amount {2}", CommandType.Global)]
    public class AddTransaction : IGlobalCommand
    {
        private Blockchain blockchain;
        private WalletProvider walletProvider;

        public AddTransaction()
        {
            this.blockchain = CommandFabric.Blockchain;
            this.walletProvider = new WalletProvider();
        }

        public string Aggregate()
        {
            return "Done";
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
            if (data.Type == SocketDataType.Send)
            {
                var transaction = JsonConvert.DeserializeObject<Transaction>(data.Body);

                this.blockchain.AddTransaction(transaction);
            }
        }

        public string Send(string[] args)
        {
            string pubKeyCompressed = CryptographyUtilities.EncodeECPointHexCompressed(walletProvider.PublicKey);
            string from = CryptographyUtilities.CalcRipeMD160(pubKeyCompressed);
            var to = args[1];
            var amount = decimal.Parse(args[2]);

            if (!this.walletProvider.HasWallet())
            {
                this.walletProvider.GenerateWallet();
            }

            var transaction = this.blockchain.AddTransaction(from, to, amount, walletProvider);

            return JsonConvert.SerializeObject(transaction);
        }
    }
}
