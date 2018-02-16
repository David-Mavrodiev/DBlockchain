using DBlockchain.Logic.Models;
using System.Collections.Generic;

namespace DBlockchain.Logic.Utils.Contracts
{
    public interface IExplorerService
    {
        List<Block> LoadBlocks(string folderPath);

        List<Transaction> LoadPendingTransactions(string path);
    }
}
