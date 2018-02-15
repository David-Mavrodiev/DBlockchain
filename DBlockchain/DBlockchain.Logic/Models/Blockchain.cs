using DBlockchain.Infrastructure.Common;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Wallet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DBlockchain.Logic.Models
{
    public class Blockchain
    {
        private IList<Node> peers;
        private readonly IList<Block> blocks;
        private IList<Transaction> pendingTransactions;

        public Blockchain()
        {
            Console.WriteLine("Start blockchain...");
            this.peers = new List<Node>();
            this.blocks = new List<Block>();
            this.pendingTransactions = new List<Transaction>();

            this.Load();

            if (this.blocks.Count == 0)
            {
                this.CreateGenesis();
            }
        }

        public IList<Node> Peers
        {
            get
            {
                return this.peers;
            }
        }

        public void CreateGenesis()
        {
            var genesisBlock = new Block();

            genesisBlock.Index = 0;
            genesisBlock.DateCreated = DateTime.Now;
            genesisBlock.Difficulty = 0;
            genesisBlock.MinedBy = "GENESIS";
            genesisBlock.Nonce = 0;
            genesisBlock.PreviousBlockHash = "GENESIS";

            var transaction = new Transaction()
            {
                From = "GENESIS",
                To = "",
                Value = 1000000000000000,
                SenderPublicKey = null,
                SenderSignature = null
            };

            string transactionJson = JsonConvert.SerializeObject(transaction);
            var transactionHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(transactionJson));

            transaction.TransactionHash = transactionHash;
            transaction.DateReceived = DateTime.Now;

            genesisBlock.Transactions = new List<Transaction>();
            genesisBlock.Transactions.Add(transaction);

            string blockJson = JsonConvert.SerializeObject(genesisBlock);
            var blockHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(blockJson));
            genesisBlock.BlockHash = blockHash;

            string path = $"{Constants.BlocksFilePath}/block_0.json";
            StorageFileProvider<Block>.SetModel(path, genesisBlock);
        }

        public void Load()
        {
            int index = 0;
            string path = $"{Constants.BlocksFilePath}/block_{index}.json";
            bool isFileExist = File.Exists(path);

            while (isFileExist)
            {
                var block = StorageFileProvider<Block>.GetModel(path);
                this.blocks.Add(block);
                index++;

                path = $"{Constants.BlocksFilePath}/block_{index}.json";
                isFileExist = File.Exists(path);
            }

            Console.WriteLine("Load blocks...");

            if (StorageFileProvider<Node[]>.HasContent(Constants.PeersFilePath))
            {
                this.peers = StorageFileProvider<Node[]>.GetModel(Constants.PeersFilePath).ToList();
            }

            Console.WriteLine("Load peers...");

            if (StorageFileProvider<Transaction[]>.HasContent(Constants.PendingTransactionsFilePath))
            {
                this.pendingTransactions = StorageFileProvider<Transaction[]>.GetModel(Constants.PendingTransactionsFilePath).ToList();
            }

            Console.WriteLine("Load pending transactions...");
        }

        public bool RegisterNode(string ip, int port)
        {
            IPAddress address;
            if (IPAddress.TryParse(ip, out address))
            {
                this.peers.Add(new Node()
                {
                    Address = ip,
                    Port = port
                });

                StorageFileProvider<Node[]>.SetModel(Constants.PeersFilePath, this.peers.ToArray());

                return true;
            }

            return false;
        }

        public Transaction AddTransaction(string from, string to, decimal amount, WalletProvider walletProvider)
        {
            var transaction = new Transaction()
            {
                From = from,
                To = to,
                Value = amount
            };

            string transactionJson = JsonConvert.SerializeObject(transaction);
            var transactionHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(transactionJson));

            var signiture = walletProvider.SignTransaction(Encoding.UTF8.GetBytes(transactionHash));
            transaction.TransactionHash = transactionHash;
            transaction.SenderSignature = signiture;

            this.pendingTransactions.Add(transaction);

            StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

            return transaction;
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            var bytes = Encoding.UTF8.GetBytes(transaction.TransactionHash);

            Console.WriteLine("Try add pending transaction...");

            if (CryptographyUtilities.VerifySigniture(bytes, transaction.SenderSignature, transaction.SenderPublicKey))
            {
                Console.WriteLine("Done...");

                this.pendingTransactions.Add(transaction);

                StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

                return transaction;
            }
            else
            {
                Console.WriteLine("Incorrect signiture...");

                return null;
            }
        }
    }
}
