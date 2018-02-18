using DBlockchain.Infrastructure.Common;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Utils;
using DBlockchain.Logic.Wallet.Contracts;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace DBlockchain.BlockExplorer.Utils
{
    public class FaucetWalletProvider : IWalletProvider
    {
        private static BigInteger privateKey;

        public FaucetWalletProvider()
        {
            UnlockWallet();
        }

        public ECPoint PublicKey
        {
            get
            {
                return CryptographyUtilities.GetPublicKeyFromPrivateKey(privateKey);
            }
        }

        public string Address
        {
            get
            {
                string pubKeyCompressed = CryptographyUtilities.EncodeECPointHexCompressed(this.PublicKey);
                string address = CryptographyUtilities.CalcRipeMD160(pubKeyCompressed);

                return address;
            }
        }

        public static void UnlockWallet()
        {
            if (privateKey == null)
            {
                var encryptedPrivateKey = StorageFileProvider<string>.GetModel(Constants.WalletEncryptedPrivateKeyFilePath);

                if (encryptedPrivateKey != null && encryptedPrivateKey != string.Empty)
                {
                    var password = "test";

                    var bytes = CryptographyUtilities.Decrypt(encryptedPrivateKey, password);

                    if (bytes != null && bytes.Length > 0)
                    {
                        privateKey = new BigInteger(bytes);
                    }
                }
            }
        }

        public byte[] SignTransaction(byte[] data)
        {
            var signiture = CryptographyUtilities.SignData(privateKey, data);

            return signiture;
        }
    }
}
