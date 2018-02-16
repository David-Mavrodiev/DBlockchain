using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Utils.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DBlockchain.Logic.Utils
{
    public class ExplorerService : IExplorerService
    {
        public List<Block> LoadBlocks(string folderPath)
        {
            int index = 0;
            string path = $"{folderPath}/block_{index}.json";
            bool isFileExist = File.Exists(path);
            List<Block> blocks = new List<Block>();

            while (isFileExist)
            {
                var block = StorageFileProvider<Block>.GetModel(path);
                blocks.Add(block);
                index++;

                path = $"{folderPath}/block_{index}.json";
                isFileExist = File.Exists(path);
            }

            return blocks;
        }

        public List<Transaction> LoadPendingTransactions(string path)
        {
            List<Transaction> pendingTransactions = StorageFileProvider<Transaction[]>.GetModel(path).ToList();

            return pendingTransactions;
        }
    }
}
