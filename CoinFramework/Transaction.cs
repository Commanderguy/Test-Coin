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
    /// <summary>
    /// The underlying transaction class, that get's hashed to verify a calculation
    /// </summary>
    /// <Todo>
    /// Add _transaction(Transaction tx) constructor. Also maybe mark as abstract and declare hash method with ((_transaction)this) to only hash these memebers
    /// </Todo>
    class _transaction
    {
        /// <summary>
        /// The sender's public key.
        /// </summary>
        public byte[] sender { get; set; }


        /// <summary>
        /// The receivers public key.
        /// </summary>
        public byte[] receiver { get; set; }


        /// <summary>
        /// The number of transactions the sender has already done.
        /// </summary>
        public int num { get; set; }


        /// <summary>
        /// The amount of coins, that get send to the receiver's address.
        /// </summary>
        public double value { get; set; }


        /// <summary>
        /// Constructor for faster initialization
        /// </summary>
        /// <param name="_sendToken">The address of the sender</param>
        /// <param name="_receiveToken">The address of the receiver</param>
        /// <param name="valueTST">The amount of coins, that get send</param>
        /// <param name="_num">The transaction number for the user</param>
        public _transaction(byte[] _sendToken, byte[] _receiveToken, double valueTST, int _num)
        {
            // Assign all values to the members
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            num = _num;
        }
    }



    /// <summary>
    /// The transaction class that includes all informations about a transaction.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The number of the transaction for the sender. After sending the transaction to the public, he increments the number
        /// </summary>
        public int num { get; set; }


        /// <summary>
        /// The public Token of the sender.
        /// </summary>
        public byte[] sender { get; set; }


        /// <summary>
        /// The public Token of the receiver.
        /// </summary>
        public byte[] receiver { get; set; }


        /// <summary>
        /// The ECDSA signature for the transaction.
        /// </summary>
        public byte[] signature { get; set; }


        /// <summary>
        /// The amount of coins, that get transferred.
        /// </summary>
        public double value { get; set; }


        /// <summary>
        /// The hash of the transaction. For memory saving, this is not included in the ToString of the Transaction, because each client can just
        /// verify the hash of the transaction by itself.
        /// </summary>
        public byte[] hash { get => SHA512.Create().ComputeHash(ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num)))); }


        /// <summary>
        /// Constructor for clients who read the signature.
        /// </summary>
        /// <param name="AsSigner">Has to be false to ensure we're constructing this class as reader only, not as signer</param>
        /// <param name="_sendToken">The public token of the sender</param>
        /// <param name="_receiveToken">The public token of the receiver</param>
        /// <param name="valueTST">The amount of coins, that get moved</param>
        /// <param name="num">The transaction number of the Signer</param>
        /// <param name="_signature">The signature of the transaction</param>
        public Transaction(bool AsSigner, byte[] _sendToken, byte[] _receiveToken, double valueTST, int num , byte[] _signature)
        {
            if (AsSigner == true) throw new Exception("Transaction should be made as verifier");
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            signature = _signature;
        }


        /// <summary>
        /// The constructor for creating a whole new transaction with the own private key
        /// </summary>
        /// <param name="_sendToken">The senders public token</param>
        /// <param name="_receiveToken">The receivers public token</param>
        /// <param name="valueTST">The amount of coins that get moved</param>
        /// <param name="num">The transaction number of the sender</param>
        /// <param name="_privateKey">The private key of the sender</param>
        public Transaction( byte[] _sendToken, byte[] _receiveToken, double valueTST, int num, byte[] _privateKey)
        {
            sender = _sendToken;
            receiver = _receiveToken;
            value = valueTST;
            
            signature = ECDSA.sign(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num)), _privateKey);
        }


        /// <summary>
        /// Default constructor (mainly implemented for newtonsoft.Json to work).
        /// </summary>
        public Transaction() { }


        /// <summary>
        /// Send the transaction to the provided list of IP's.
        /// </summary>
        /// <param name="txs">The list of IP's</param>
        public void SendTransaction(List<string> txs)
        {
            throw new NotImplementedException();
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


        /// <summary>
        /// Returns true if the transaction is valid. Verifies transactionnumber, signature, and if the sender even has the provided funds. 
        /// </summary>
        /// <param name="bc">The blockchain, to get the funds of the senders token.</param>
        /// <returns></returns>
        public bool isValid(Blockchain bc)
        {
            foreach(var x in bc.chain)
            {
                foreach(var n in x.transactions)
                {
                    // Implement num counter
                }
            }
            // Verify the transaction with the public token
            if(!ECDSA.verify(JsonConvert.SerializeObject(new _transaction(sender, receiver, value, num)), signature, sender)) return false;
            return bc.count_funds(sender) > value;
        }
    }
}
