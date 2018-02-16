using DBlockchain.BlockExplorer.Utils;
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
            var pendingTransactions = exploreService.LoadPendingTransactions(Constants.PendingTransactionsPath);
            var blocks = this.exploreService.LoadBlocks(Constants.BlocksFolder);
            blocks.Reverse();

            return View(blocks.ToList());
        }

        public IActionResult Details(int id)
        {
            var block = this.exploreService.LoadBlocks(Constants.BlocksFolder).First(b => b.Index == id);

            return View(block);
        }
    }
}
