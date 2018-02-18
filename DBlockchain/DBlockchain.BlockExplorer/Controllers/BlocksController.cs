using DBlockchain.BlockExplorer.Utils;
using DBlockchain.Infrastructure.Common;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Utils.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DBlockchain.BlockExplorer.Controllers
{
    public class BlocksController : Controller
    {
        private IExplorerService exploreService;

        public BlocksController()
        {
            this.exploreService = new ExplorerService();
        }

        public IActionResult Index()
        {
            var pendingTransactions = exploreService.LoadPendingTransactions(Constants.PendingTransactionsFilePath);
            var blocks = this.exploreService.LoadBlocks(Constants.BlocksFilePath);
            blocks.Reverse();

            return View(blocks.ToList());
        }

        public IActionResult Details(int id)
        {
            var block = this.exploreService.LoadBlocks(Constants.BlocksFilePath).First(b => b.Index == id);

            return View(block);
        }
    }
}
