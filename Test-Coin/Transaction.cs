using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Coin
{
    public class Transaction
    {
        public byte[] sender { get; set; }
        public byte[] receiver { get; set; }

        public double value { get; set; }

        public Transaction(byte[] _sendToken, byte[] _receiveToken, double valueTST)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
        }


        public bool isValid(Blockchain bc)
        {
            return bc.count_funds(sender) > value;
        }
    }
}
