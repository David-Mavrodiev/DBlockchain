using Org.BouncyCastle.Math.EC;

namespace DBlockchain.Logic.Wallet.Contracts
{
    public interface IWalletProvider
    {
        ECPoint PublicKey { get; }

        string Address { get; }

        byte[] SignTransaction(byte[] data);
    }
}
