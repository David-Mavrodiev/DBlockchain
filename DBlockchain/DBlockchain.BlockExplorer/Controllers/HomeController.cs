using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DBlockchain.BlockExplorer.Models;
using System.Linq;
using DBlockchain.Logic.Utils;
using DBlockchain.BlockExplorer.Utils;
using DBlockchain.Logic.Utils.Contracts;
using DBlockchain.Infrastructure.Common;

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
            var pendingTransactions = this.exploreService.LoadPendingTransactions(Constants.PendingTransactionsFilePath);
            var blocks = exploreService.LoadBlocks(Constants.BlocksFilePath);
            blocks.Reverse();
            pendingTransactions.Reverse();

            var viewModel = new HomeViewModel()
            {
                Blocks = blocks.Take(5).ToList(),
                PendingTransactions = pendingTransactions.Take(5).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
