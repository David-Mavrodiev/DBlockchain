using System;
using System.IO;
using System.Linq;

namespace DBlockchain.Infrastructure.Common
{
    public static class Constants
    {
        static Constants()
        {
            var dir = Environment.CurrentDirectory.Replace('\\', '/').Split('/').ToList();
            var storagePath = string.Empty;

            for (int i = 0; i < dir.Count; i++)
            {
                var path = string.Join("/", dir.Take(dir.Count - i));

                if (Directory.Exists($"{path}/Storage"))
                {
                    storagePath = $"{path}/Storage";
                    break;
                }
            }

            WalletEncryptedPrivateKeyFilePath = $"{storagePath}/Wallet/encryptedPrivateKey.json";
            BlocksFilePath = $"{storagePath}/Blocks";
            PeersFilePath = $"{storagePath}/peers.json";
            PendingTransactionsFilePath = $"{storagePath}/pendingTransactions.json";
        }

        public static string WalletEncryptedPrivateKeyFilePath;
        public static string BlocksFilePath;
        public static string PeersFilePath;
        public static string PendingTransactionsFilePath;
    }
}
