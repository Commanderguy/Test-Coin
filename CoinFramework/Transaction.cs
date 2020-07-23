using DigitalSignatureAlgo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Coin
{
    class _transaction
    {
        public byte[] sender { get; set; }
        public byte[] receiver { get; set; }


        public double value { get; set; }

        public _transaction(byte[] _sendToken, byte[] _receiveToken, double valueTST)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
        }
    }




    public class Transaction
    {
        public byte[] sender { get; set; }
        public byte[] receiver { get; set; }

        public byte[] signature { get; }

        public double value { get; set; }

        public Transaction(bool AsSigner, byte[] _sendToken, byte[] _receiveToken, double valueTST, byte[] _signature)
        {
            if (AsSigner == true) throw new Exception("Transaction should be made as verifier");
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            signature = _signature;
        }

        public Transaction( byte[] _sendToken, byte[] _receiveToken, double valueTST, byte[] _privateKey)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            signature = ECDSA.sign(JsonConvert.SerializeObject(new _transaction(sender, receiver, value)), _privateKey);
        }

        public bool isValid(Blockchain bc)
        {
            ECDSA.verify(JsonConvert.SerializeObject(new _transaction(sender, receiver, value)), signature, sender);
            return bc.count_funds(sender) > value;
        }
    }
}
