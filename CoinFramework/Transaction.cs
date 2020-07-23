using DigitalSignatureAlgo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Coin
{
    class _transaction
    {
        public byte[] sender { get; set; }
        public byte[] receiver { get; set; }
        public int num { get; set; }
        public double value { get; set; }

        public _transaction(byte[] _sendToken, byte[] _receiveToken, double valueTST, int _num)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            num = _num;
        }
    }




    public class Transaction
    {
        public int num { get; set; }
        public byte[] sender { get; set; }
        public byte[] receiver { get; set; }

        public byte[] signature { get; }

        public double value { get; set; }

        public byte[] hash { get; }

        public Transaction(bool AsSigner, byte[] _sendToken, byte[] _receiveToken, double valueTST, int num , byte[] _signature)
        {
            if (AsSigner == true) throw new Exception("Transaction should be made as verifier");
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            signature = _signature;
        }

        public Transaction( byte[] _sendToken, byte[] _receiveToken, double valueTST, int num, byte[] _privateKey)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            
            signature = ECDSA.sign(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num)), _privateKey);
            hash = SHA512.Create().ComputeHash(ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num))));
        }

        public Transaction() { }

        public void SendTransaction(List<string> txs)
        {
            Thread th = new Thread(() =>
            {
                var NEWTRANSACTION = this;
                foreach (var n in txs)
                {
                    TcpClient client = new TcpClient(n, 8080);
                    var stream = client.GetStream();
                    var send = ASCIIEncoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(NEWTRANSACTION));
                    stream.Write(send, 0, send.Length);
                    client.Close();
                }
            });
            th.Start();
        }


        public bool isValid(Blockchain bc)
        {
            foreach(var x in bc.chain)
            {
                foreach(var n in x.transactions)
                {
                    // Implement num counter
                }
            }
            ECDSA.verify(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num)), signature, sender);
            return bc.count_funds(sender) > value;
        }
    }
}
