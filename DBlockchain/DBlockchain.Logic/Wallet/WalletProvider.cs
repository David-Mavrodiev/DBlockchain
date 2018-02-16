using DBlockchain.Infrastructure.Common;
using DBlockchain.Infrastructure.Network;
using DBlockchain.Logic.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;

namespace DBlockchain.Logic.Wallet
{
    public class WalletProvider
    {
        private static BigInteger privateKey;

        public WalletProvider()
        {
            UnlockWallet();
        }

        public static void UnlockWallet()
        {
            if (privateKey == null)
            {
                var encryptedPrivateKey = StorageFileProvider<string>.GetModel(Constants.WalletEncryptedPrivateKeyFilePath);

                if (encryptedPrivateKey != null && encryptedPrivateKey != string.Empty)
                {
                    Console.WriteLine("Type your password:");
                    var password = Console.ReadLine();

                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("Unlock wallet...");
                            var bytes = CryptographyUtilities.Decrypt(encryptedPrivateKey, password);

                            if (bytes != null && bytes.Length > 0)
                            {
                                privateKey = new BigInteger(bytes);
                            }

                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Incorrect password...");
                            Console.WriteLine("Type your password again:");
                            password = Console.ReadLine();
                        }
                    }
                }
            }
        }

        public bool HasWallet()
        {
            if (privateKey != null && privateKey != BigInteger.Zero)
            {
                return true;
            }

            return false;
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

        public byte[] SignTransaction(byte[] data)
        {
            var signiture = CryptographyUtilities.SignData(privateKey, data);

            return signiture;
        }

        private static void GenerateIdentity(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();
            SecureRandom secureRandom = new SecureRandom();
            KeyGenerationParameters keyGenParam =
                new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);

            var keyPair = gen.GenerateKeyPair();

            BigInteger privateKey = ((ECPrivateKeyParameters)keyPair.Private).D;
            Console.WriteLine("Private key (hex): " + privateKey.ToString(16));
            Console.WriteLine("Private key: " + privateKey.ToString(10));

            Console.WriteLine("Password for private key encryption:");
            var password = Console.ReadLine();
            var encryptedPrivateKey = CryptographyUtilities.Encrypt(privateKey.ToByteArray(), password);

            StorageFileProvider<string>.SetModel(Constants.WalletEncryptedPrivateKeyFilePath, encryptedPrivateKey);

            ECPoint pubKey = ((ECPublicKeyParameters)keyPair.Public).Q;

            string pubKeyCompressed = CryptographyUtilities.EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + pubKeyCompressed);

            string addr = CryptographyUtilities.CalcRipeMD160(pubKeyCompressed);
            Console.WriteLine("Blockchain address: " + addr);
        }

        public void GenerateWallet()
        {
            Console.WriteLine("Generate keys...");

            GenerateIdentity();
        }
    }
}
