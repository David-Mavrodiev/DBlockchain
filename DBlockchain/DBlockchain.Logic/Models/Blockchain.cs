using DBlockchain.Infrastructure.Common;
using DBlockchain.Logic.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DBlockchain.Logic.Models
{
    public class Blockchain
    {
        private readonly IList<Node> peers;
        private readonly IList<Block> blocks;

        public Blockchain()
        {
            Console.WriteLine("Load blockchain...");
            this.peers = new List<Node>();
            this.blocks = new List<Block>();

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
                SenderPublicKey = "GENESIS",
                SenderSignature = "GENESIS"
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
            File.WriteAllText(path, JsonConvert.SerializeObject(genesisBlock));
        }

        public void Load()
        {
            int index = 0;
            string path = $"{Constants.BlocksFilePath}/block_{index}.json";
            bool isFileExist = File.Exists(path);

            while (isFileExist)
            {
                var content = File.ReadAllText(path);
                var block = JsonConvert.DeserializeObject<Block>(content);
                this.blocks.Add(block);
                index++;

                path = $"{Constants.BlocksFilePath}/block_{index}.json";
                isFileExist = File.Exists(path);
            }
        }
    }
}
