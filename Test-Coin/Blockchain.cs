using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Coin
{
    [Serializable]
    public class Blockchain
    {

        public List<Block> chain = new List<Block>();

        public bool validate()
        {
            for(int i = 0; i < chain.Count; i++)
            {
                if (!chain[i].isValid((chain[i].block_number / Environment.diffReducer) + Environment.initialDifficulty)) return false;
            }
            return true;
        }

        public double count_funds(byte[] address)
        {
            double start = 0;
            foreach(var bx in chain)
            {
                Console.Write("Testing if ");
                foreach (var n in address)
                    Console.Write(n);
                Console.Write(" is equal to ");
                foreach (var n in bx.miner)
                    Console.Write(n);
                


                if (Enumerable.SequenceEqual(address, bx.miner))
                {
                    start += Environment.InitialCoinPerBlock / ((bx.block_number / Environment.diffReducer) + 1);
                    Console.WriteLine("\nIf yes, this message should be visible");
                }
                else
                {
                    Console.WriteLine("\nIf it is not equal, you should see this");
                }
                foreach (var tx in bx.transactions)
                {
                    if(Enumerable.SequenceEqual(address, tx.receiver))
                    {
                        start += tx.value;
                    }
                    if(Enumerable.SequenceEqual(address, tx.sender))
                    {
                        start -= tx.value;
                    }
                    
                }
            }
            return start;
        }


        public byte[] getLastHash() => chain[chain.Count - 1].hash;


        public void AddBlock(Block b)
        {
            chain.Add(b);
        }

        

    }
}
