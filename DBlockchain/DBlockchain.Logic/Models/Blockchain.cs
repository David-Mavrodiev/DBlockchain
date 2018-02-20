using DBlockchain.Infrastructure.Common;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Wallet;
using DBlockchain.Logic.Wallet.Contracts;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace DBlockchain.Logic.Models
{
    public class Blockchain
    {
        private IDictionary<string, decimal> balances { get; set; }
        private IList<Node> peers;
        private readonly IList<Block> blocks;
        private IList<Transaction> pendingTransactions;
        private readonly int difficulty = 5;

        public Blockchain()
        {
            Console.WriteLine("Start blockchain...");
            this.peers = new List<Node>();
            this.blocks = new List<Block>();
            this.pendingTransactions = new List<Transaction>();
            this.balances = new Dictionary<string, decimal>();

            this.Load();

            if (this.blocks.Count == 0)
            {
                this.CreateGenesis();
            }

            this.CalculateBalances(0);
        }

        public int Difficulty
        {
            get
            {
                return this.difficulty;
            }
        }

        public Block LastBlock
        {
            get
            {
                return this.blocks.Last();
            }
        }

        public List<Block> Blocks
        {
            get
            {
                return this.blocks.ToList();
            }
        }

        public IList<Node> Peers
        {
            get
            {
                return this.peers;
            }
        }

        public IDictionary<string, decimal> GetBalances(int confirmations)
        {
            this.CalculateBalances(confirmations);

            return this.balances;
        }

        public void RemoveBlockInterval(int from, int to)
        {
            Console.WriteLine($"Romove blocks from {from} to {to}...");
            int lastBlockIndex = this.LastBlock.Index;

            for (int i = from; i <= to; i++)
            {
                if (i > lastBlockIndex)
                {
                    break;
                }

                if (i == 0)
                {
                    continue;
                }

                var oldBlock = this.blocks.First(b => b.Index == i);
                this.blocks.Remove(oldBlock);

                File.Delete($"{Constants.BlocksFilePath}/block_{i}.json");
            }
        }

        private void CalculateBalances(int confirmations)
        {
            var balances = new Dictionary<string, decimal>();

            foreach (var genesisTransactions in blocks[0].Transactions)
            {
                balances.Add(genesisTransactions.To, genesisTransactions.Value);
            }

            foreach (var block in blocks.Skip(1).Take(blocks.Count - confirmations))
            {
                foreach (var transaction in block.Transactions)
                {
                    var senderPublicKey = CryptographyUtilities.DecodeECPointFromHex(transaction.SenderPublicKey);
                    var bytes = Encoding.UTF8.GetBytes(transaction.TransactionHash);

                    if (CryptographyUtilities.VerifySigniture(bytes, transaction.SenderSignature, senderPublicKey))
                    {
                        balances[transaction.From] -= transaction.Value;

                        if (balances.ContainsKey(transaction.To))
                        {
                            balances[transaction.To] += transaction.Value;
                        }
                        else
                        {
                            balances.Add(transaction.To, transaction.Value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Found incorrect signiture...");
                        return;
                    }
                }
            }

            this.balances = balances;
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
                To = "e39f2a9daf79084f96b28d0b92439b0b6112981c",
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

            this.blocks.Add(genesisBlock);
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

        public Block AddBlock(int nonce, WalletProvider walletProvider)
        {
            string lastBlockHash = this.LastBlock.BlockHash;
            string winnerHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256($"{lastBlockHash}{nonce}"));

            if (!winnerHash.ToCharArray().Take(this.Difficulty).All(s => s == '0'))
            {
                Console.WriteLine("Incorrect winner hash...");
                return null;
            }

            var transactions = this.pendingTransactions;

            foreach (var transaction in transactions)
            {
                transaction.DateReceived = DateTime.Now;
                transaction.MinedInBlockIndex = this.LastBlock.Index + 1;
            }

            var block = new Block()
            {
                Index = this.LastBlock.Index + 1,
                DateCreated = DateTime.Now,
                Difficulty = this.Difficulty,
                MinedBy = walletProvider.Address,
                Nonce = nonce,
                PreviousBlockHash = this.LastBlock.BlockHash,
                Transactions = transactions.ToList()
            };

            string blockJson = JsonConvert.SerializeObject(block);
            var blockHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(blockJson));
            block.BlockHash = blockHash;

            this.blocks.Add(block);
            StorageFileProvider<Block>.SetModel($"{Constants.BlocksFilePath}/block_{block.Index}.json", block);

            //Shoud think :-)
            this.pendingTransactions = new List<Transaction>();
            StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

            return block;
        }

        public Block AddBlock(Block block)
        {
            Console.WriteLine("Try add block...");

            if (ValidateBlock(block))
            {
                this.blocks.Add(block);
                StorageFileProvider<Block>.SetModel($"{Constants.BlocksFilePath}/block_{block.Index}.json", block);

                //Shoud think :-)
                this.pendingTransactions = new List<Transaction>();
                StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

                Console.WriteLine("Done...");

                return block;
            }

            return null;
        }

        public Transaction AddTransaction(string from, string to, decimal amount, IWalletProvider walletProvider)
        {
            this.CalculateBalances(0);

            if (balances.ContainsKey(from) && balances[from] < amount)
            {
                Console.WriteLine("Not enought coins...");
                return null;
            }

            var publicKeyHex = CryptographyUtilities.BytesToHex(walletProvider.PublicKey.GetEncoded());
            var transaction = new Transaction()
            {
                    From = from,
                    To = to,
                    Value = amount,
                    SenderPublicKey = publicKeyHex
            };

            string transactionJson = JsonConvert.SerializeObject(transaction, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            var transactionHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(transactionJson));

            var signiture = walletProvider.SignTransaction(Encoding.UTF8.GetBytes(transactionHash));
            transaction.TransactionHash = transactionHash;
            transaction.SenderSignature = signiture;

            this.pendingTransactions.Add(transaction);

            StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

            Console.WriteLine("Transaction is pending...");

            return transaction;
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            Console.WriteLine("Try add pending transaction...");

            if (ValidateTransaction(transaction))
            {
                Console.WriteLine("Done...");

                this.pendingTransactions.Add(transaction);
                StorageFileProvider<Transaction[]>.SetModel(Constants.PendingTransactionsFilePath, this.pendingTransactions.ToArray());

                return transaction;
            }
            
            return null;
        }

        private bool ValidateTransaction(Transaction transaction, IDictionary<string, decimal> futureBalances = null)
        {
            IDictionary<string, decimal> balances;
            bool inSimulationMode = false;

            if (futureBalances != null)
            {
                inSimulationMode = true;
                balances = futureBalances;
            }
            else
            {
                balances = GetBalances(0);
            }

            var bytes = Encoding.UTF8.GetBytes(transaction.TransactionHash);

            ECPoint senderPublicKey = CryptographyUtilities.DecodeECPointFromHex(transaction.SenderPublicKey);

            if (!CryptographyUtilities.VerifySigniture(bytes, transaction.SenderSignature, senderPublicKey))
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("Transaction validation failed...");
                Console.WriteLine("INFO -> incorrect signiture...");
                return false;
            }

            if (!balances.ContainsKey(transaction.From) || balances[transaction.From] < transaction.Value)
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("Transaction validation failed...");
                Console.WriteLine("INFO -> not enought money of sender...");
                return false;
            }

            if (inSimulationMode)
            {
                balances[transaction.From] -= transaction.Value;
            }

            var copyOftransaction = new Transaction()
            {
                From = transaction.From,
                To = transaction.To,
                Value = transaction.Value,
                SenderPublicKey = transaction.SenderPublicKey
            };

            var transactionJson = JsonConvert.SerializeObject(copyOftransaction);
            var transactionHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(transactionJson));

            if (transactionHash != transaction.TransactionHash)
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("Transaction validation failed...");
                Console.WriteLine("INFO -> incorrect transaction hash...");
                return false;
            }

            Console.WriteLine("Transaction validation success...");

            return true;
        }

        public bool ValidateBlock(Block block, IDictionary<string, decimal> futureBalances = null)
        {
            Console.WriteLine("Validate block...");

            int nonce = block.Nonce;
            string lastBlockHash = this.LastBlock.BlockHash;
            string winnerHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256($"{lastBlockHash}{nonce}"));

            if (!winnerHash.ToCharArray().Take(this.Difficulty).All(s => s == '0'))
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("Block validation failed...");
                Console.WriteLine("INFO -> Incorrect winner hash...");
                Console.WriteLine($"Block number: {block.Index}, last block: {this.LastBlock.Index}");

                return false;
            }

            var copyOfBlock = new Block()
            {
                Index = block.Index,
                DateCreated = block.DateCreated,
                Difficulty = block.Difficulty,
                MinedBy = block.MinedBy,
                Nonce = block.Nonce,
                PreviousBlockHash = block.PreviousBlockHash,
                Transactions = block.Transactions
            };

            string blockJson = JsonConvert.SerializeObject(copyOfBlock);
            var blockHash = CryptographyUtilities.BytesToHex(CryptographyUtilities.CalcSHA256(blockJson));

            if (blockHash != block.BlockHash)
            {
                Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                Console.WriteLine("Block validation failed...");
                Console.WriteLine("INFO -> Incorrect block hash...");
                return false;
            }

            IDictionary<string, decimal> balances;

            if (futureBalances != null)
            {
                balances = futureBalances;
            }
            else
            {
                balances = GetBalances(0);
            }

            foreach (var transaction in block.Transactions)
            {
                if (ValidateTransaction(transaction, balances))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidateChain(IList<Block> blocks)
        {
            Console.WriteLine("Validate chain...");

            var balances = GetBalances(0);

            for (int i = 0; i < blocks.Count; i++)
            {
                if (!ValidateBlock(blocks[i], balances))
                {
                    Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                    Console.WriteLine("Chain is not valid...");
                    return false;
                }

                if (i > 0 && blocks[i - 1].BlockHash != blocks[i].PreviousBlockHash)
                {
                    Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^");
                    Console.WriteLine("Block validation failed...");
                    Console.WriteLine("INFO -> Hashes between two blocks is not valid...");
                    return false;
                }
            }

            Console.WriteLine("Chain is valid...");
            return true;
        }
    }
}
