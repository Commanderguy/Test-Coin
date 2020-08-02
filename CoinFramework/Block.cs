using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using System.Web.Security;

namespace CoinFramework
{
    /// <summary>
    /// The underlying Block class, that get's hashed to verify a calculation.
    /// </summary>
    /// <Todo>
    /// Add _blockHash(Block b) constructor. Also maybe mark as abstract and declare hash method with ((_blockHash)this) to only hash these memebers.
    /// </Todo>
    [Serializable]
    class _blockHash
    {
        /// <summary>
        /// The list of transactions.
        /// </summary>
        //public List<Transaction> transactions = new List<Transaction>();


        /// <summary>
        /// The hash of the previous block. This ensures, that a miner can't just publish a block with 0 transactions multiple times.
        /// </summary>
        public byte[] prev_hash;


        /// <summary>
        /// The nonce of the block. With this, we can ensure, that the miner has worked some time for this.
        /// </summary>
        public string nonce;


        /// <summary>
        /// The number of the block in the blockchain.
        /// Given index i for blockchain[i].blocknumber == i.
        /// </summary>
        public long block_number;


        /// <summary>
        /// The public token of the miner.
        /// </summary>
        public byte[] miner;


        /// <summary>
        /// Default constructor for simpler initialisation and to save cpu time for faster hashing (later also on gpu).
        /// </summary>
        /// <param name="txs">The transactions</param>
        /// <param name="_prev_hash">The hash of the previous block</param>
        /// <param name="_nonce">The nonce of the block</param>
        /// <param name="block_num">The number of the block</param>
        public _blockHash(List<Transaction> txs, byte[] _prev_hash, string _nonce, long block_num, byte[] _miner)
        {
            // Assign paramters to memebrs
            //transactions = txs;
            prev_hash = _prev_hash;
            nonce = _nonce;
            block_number = block_num;
            miner = _miner;
        }
    }




    /// <summary>
    /// The public Block class allows the creating of new blocks or parsing existing ones in the blockchain. Most important 
    /// </summary>
    public class Block
    {
        public List<Transaction> transactions = new List<Transaction>();
        public byte[] prev_hash;
        public byte[] hash
        {
            get => _toHash();
        }
        public long block_number;
        public string nonce;

        static SHA512 sha = SHA512.Create();
        byte[] _toHash() => sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ToString()));

        public byte[] miner;

        public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(new _blockHash(transactions, prev_hash, nonce, block_number, miner));


        public bool isValid(long diff, Blockchain bc)
        {
            for(int i = 0; i < diff; i++)
            {
                if (hash[i] != 0)
                {
                    return false;
                }
            }
            foreach(var tx in transactions)
            {
                if (!tx.isValid(bc, block_number)) return false;
            }
            return true;
        }

        public void calculateNonce(byte[] _miner, Blockchain chain, ref bool stop)
        {
            miner = _miner;
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stop)
            {
                nonce = Membership.GeneratePassword(32, 0);
                for (int i = 0; i < (block_number / Environment.diffReducer) + Environment.initialDifficulty; i++)
                {
                    if (hash[i] != 0) goto _end;
                }
                
                stop = false;
                Console.WriteLine("Found nonce: " + nonce);
            _end:;
            }
            stopWatch.Stop();
            Console.WriteLine("Found block after " + stopWatch.Elapsed.Hours + "h" + stopWatch.Elapsed.Minutes + "m" + stopWatch.Elapsed.Seconds + "s" + stopWatch.Elapsed.Milliseconds + "ms");
            if (!isValid((block_number / Environment.diffReducer) + Environment.initialDifficulty, chain)) throw new Exception("Nonce was stopped before finishing");
        }


        public void AddTransaction(Transaction tx)
        {
            transactions.Add(tx);
        }

        public void AddTransaction(byte[] sender, byte[] receiver, double amount, byte[] signature, int num)
        {
            Transaction tx = new Transaction(false, sender, receiver, amount, num, signature);
            transactions.Add(tx);
        }


        public void AddNewTransactionEvaluateSignature(byte[] sender, byte[] receiver, double amount, int num, byte[] privateKey)
        {
            Transaction tx = new Transaction(sender, receiver, amount, num, privateKey);
            transactions.Add(tx);
        }


        public Block(List<Transaction> txs)
        {
            transactions = txs;
        }

        public Block() { }

    }
}
