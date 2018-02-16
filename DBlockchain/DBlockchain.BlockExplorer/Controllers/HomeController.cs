using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DBlockchain.BlockExplorer.Models;
using System.Linq;
using DBlockchain.Logic.Utils;
using DBlockchain.BlockExplorer.Utils;
using DBlockchain.Logic.Utils.Contracts;

namespace DBlockchain.BlockExplorer.Controllers
{
    public class HomeController : Controller
    {
        private IExplorerService exploreService;

        public HomeController()
        {
            this.exploreService = new ExplorerService();
        }

        public IActionResult Index()
        {
            var pendingTransactions = this.exploreService.LoadPendingTransactions(Constants.PendingTransactionsPath);
            var blocks = exploreService.LoadBlocks(Constants.BlocksFolder);
            blocks.Reverse();

            var viewModel = new HomeViewModel()
            {
                Blocks = blocks.Take(5).ToList(),
                PendingTransactions = pendingTransactions
            };

            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
