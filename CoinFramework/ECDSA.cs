using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignatureAlgo
{
    public struct KeyPair
    {
        public byte[] pubKey;
        public byte[] privKey;

        public KeyPair(byte[] publicKey, byte[] privateKey)
        {
            pubKey = publicKey;
            privKey = privateKey;
        }
    }



    public class ECDSA
    {
        static public byte[] sign(string msg, byte[] privateKey)
        {
            using (ECDsaCng algo = new ECDsaCng(CngKey.Import(privateKey, CngKeyBlobFormat.EccPrivateBlob)))
            {
                return algo.SignData(Encoding.ASCII.GetBytes(msg), HashAlgorithmName.SHA512);
            }
        }

        static public bool verify(string msg, byte[] signature, byte[] publicKey)
        {
            try
            {
                using (ECDsaCng algo = new ECDsaCng(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob)))
                {
                    return algo.VerifyData(ASCIIEncoding.ASCII.GetBytes(msg), signature, HashAlgorithmName.SHA512);
                }
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return false;
            }
        }


        /// <summary>
        /// Generate a new Key pair (experimental, rng implemented by system)
        /// </summary>
        /// <returns></returns>
        static public KeyPair GenerateKeys()
        {
            using (ECDsaCng algo = new ECDsaCng())
            {
                byte[] pubKey = algo.Key.Export(CngKeyBlobFormat.EccPublicBlob);
                byte[] privKey = algo.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
                return new KeyPair(pubKey, privKey);
            }
        }
    }
}
