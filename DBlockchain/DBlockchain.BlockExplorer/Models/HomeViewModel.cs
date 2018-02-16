using DBlockchain.Logic.Models;
using System.Collections.Generic;

namespace DBlockchain.BlockExplorer.Models
{
    public class HomeViewModel
    {
        public List<Transaction> PendingTransactions { get; set; }

        public List<Block> Blocks { get; set; }
    }
}
