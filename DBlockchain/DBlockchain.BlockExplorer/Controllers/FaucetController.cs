using DBlockchain.BlockExplorer.Models;
using DBlockchain.BlockExplorer.Utils;
using DBlockchain.Logic.Models;
using DBlockchain.Logic.Wallet.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DBlockchain.BlockExplorer.Controllers
{
    public class FaucetController : Controller
    {
        private Blockchain blockchain;
        private IWalletProvider faucetWalletProvider;

        public FaucetController()
        {
            this.blockchain = new Blockchain();
            this.faucetWalletProvider = new FaucetWalletProvider();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(FaucetTransactionViewModel viewModel)
        {
            this.blockchain.AddTransaction(this.faucetWalletProvider.Address, viewModel.To, 5, this.faucetWalletProvider);

            return RedirectToAction("Index", "Home");
        }
    }
}
