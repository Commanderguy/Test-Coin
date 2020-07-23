using DigitalSignatureAlgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFramework
{
    public class AccountView
    {
        public KeyPair keys { get; set; }
        public byte[] publicKey => keys.pubKey;
        public byte[] privateKey => keys.privKey;
        public AccountView()
        {
            keys = ECDSA.GenerateKeys();
        }
    }
}
