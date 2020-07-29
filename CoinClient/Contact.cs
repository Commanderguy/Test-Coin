using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinClient
{
    public class Contact
    {
        public string name { get; set; }
        public byte[] pubToken { get; set; }
        public double balance { get; set; }

        public Contact(string sName, byte[] bToken, double dBalance)
        {
            name = sName;
            pubToken = bToken;
            balance = dBalance;
        }
    }
}
