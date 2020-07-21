using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Test_Coin
{
    [Serializable]
    class _blockHash
    {
        public List<Transaction> transactions = new List<Transaction>();
        public byte[] prev_hash;
        public string nonce;
        public long block_number;
        public _blockHash(List<Transaction> txs, byte[] _prev_hash, string _nonce, long block_num)
        {
            transactions = txs;
            prev_hash = _prev_hash;
            nonce = _nonce;
            block_number = block_num;
        }
    }




    
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

        public bool mined = false;
        static SHA512 sha = SHA512.Create();
        byte[] _toHash() => sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ToString()));

        public byte[] miner;

        public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(new _blockHash(transactions, prev_hash, nonce, block_number));


        public bool isValid(long diff)
        {
            for(int i = 0; i < diff; i++)
            {
                if (hash[i] != 0) return false;
            }
            return true;
        }

        public void calculateNonce(byte[] _miner)
        {
            long tVal = 0;
            bool cond = true;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (cond)
            {
                tVal++;
                var n = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new _blockHash(transactions, prev_hash, Convert.ToString(tVal), block_number))));
                for (int i = 0; i < (block_number / Environment.diffReducer) + Environment.initialDifficulty; i++)
                {
                    if (n[i] != 0) goto _end;
                }
                
                cond = false;
                nonce = Convert.ToString(tVal);
                Console.WriteLine("Found nonce: " + nonce);
            _end:;
            }
            stopWatch.Stop();
            Console.WriteLine("Found block after " + stopWatch.Elapsed.Hours + "h" + stopWatch.Elapsed.Minutes + "m" + stopWatch.Elapsed.Seconds + "s" + stopWatch.Elapsed.Milliseconds + "ms");
            miner = _miner;
            mined = true;
            if (!isValid((block_number / Environment.diffReducer) + Environment.initialDifficulty)) throw new Exception("Nonce was stopped before finishing");
        }


        public void AddTransaction(Transaction tx)
        {
            transactions.Add(tx);
        }

        public void AddTransaction(byte[] sender, byte[] receiver, double amount)
        {
            Transaction tx = new Transaction(sender, receiver, amount);
            transactions.Add(tx);
        }


        public Block(List<Transaction> txs)
        {
            transactions = txs;
        }

        public Block() { }

    }
}
