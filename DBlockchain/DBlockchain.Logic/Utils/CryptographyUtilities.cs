using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DBlockchain.Logic.Utils
{
    public static class CryptographyUtilities
    {
        public static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        public static Random rand = new Random();

        public static ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            ECPoint pubKey = curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        public static byte[] SignData(BigInteger privateKey, byte[] data)
        {
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            ECPrivateKeyParameters keyParameters = new ECPrivateKeyParameters(privateKey, ecSpec);

            ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            signer.Init(true, keyParameters);
            signer.BlockUpdate(data, 0, data.Length);
            byte[] sigBytes = signer.GenerateSignature();

            return sigBytes;
        }

        public static bool VerifySigniture(byte[] data, byte[] signiture, ECPoint publicKey)
        {
            ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            ECDomainParameters ecSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            ECPublicKeyParameters keyParameters = new ECPublicKeyParameters(publicKey, ecSpec);
            signer.Init(false, keyParameters);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signiture);
        }

        public static string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public static byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        public static string CalcRipeMD160(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            RipeMD160Digest digest = new RipeMD160Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);
        }

        public static string EncodeECPointHexCompressed(ECPoint point)
        {
            BigInteger x = point.XCoord.ToBigInteger();
            return x.ToString(16) + Convert.ToInt32(!x.TestBit(0));
        }

        public static ECPoint DecodeECPointFromHex(string hex)
        {
            ECPoint point = CryptographyUtilities.curve.Curve.DecodePoint(Hex.Decode(hex)).Normalize();

            return point;
        }

        public static string Encrypt(byte[] clearBytes, string password)
        {
            string encryptedBytes = string.Empty;

            using (Aes encryptor = Aes.Create())
            {
                byte[] IV = new byte[15];    
                rand.NextBytes(IV);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = Convert.ToBase64String(IV) + Convert.ToBase64String(ms.ToArray());
                }
            }

            return encryptedBytes;
        }

        public static byte[] Decrypt(string encryptedBytes, string password)
        {
            byte[] bytes = null;
            byte[] IV = Convert.FromBase64String(encryptedBytes.Substring(0, 20));
            encryptedBytes = encryptedBytes.Substring(20).Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encryptedBytes);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }
    }
}
