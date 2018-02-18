using DBlockchain.BlockExplorer.Utils;
using DBlockchain.Infrastructure.Common;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Utils.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace DBlockchain.BlockExplorer.Controllers
{
    public class TransactionsController : Controller
    {
        private IExplorerService exploreService;

        public TransactionsController()
        {
            this.exploreService = new ExplorerService();
        }

        public IActionResult Index()
        {
            var blocks = this.exploreService.LoadBlocks(Constants.BlocksFilePath);
            List<Transaction> transactions = new List<Transaction>();

            foreach (var block in blocks)
            {
                transactions.AddRange(block.Transactions);
            }

            transactions.Reverse();

            return View(transactions);
        }

        public IActionResult Details(string id)
        {
            var blocks = this.exploreService.LoadBlocks(Constants.BlocksFilePath);
            List<Transaction> transactions = new List<Transaction>();

            foreach (var block in blocks)
            {
                transactions.AddRange(block.Transactions);
            }

            var transaction = transactions.First(t => t.TransactionHash == id);

            return View(transaction);
        }
    }
}
