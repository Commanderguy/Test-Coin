using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinFramework
{
    /// <summary>
    /// The blockchain class connects the List of blocks with methods to analyze and validate the chain.
    /// </summary>
    [Serializable]
    public class Blockchain
    {
        /// <summary>
        /// The list of blocks.
        /// </summary>
        public List<Block> chain = new List<Block>();


        /// <summary>
        /// Test if the chain is valid or not.
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
            for(int i = 0; i < chain.Count; i++)
            {
                if (!chain[i].isValid((chain[i].block_number / Environment.diffReducer) + Environment.initialDifficulty, this)) return false;
            }
            return true;
        }


        /// <summary>
        /// Count the funds of $address.
        /// </summary>
        /// <param name="address">The public token of the address to count the funds</param>
        /// <returns>The amount of coins the address owns.</returns>
        public double count_funds(byte[] address)
        {
            double start = 0;
            
            foreach(var bx in chain)
            {
                //Count funds gained through mining.
                if (Enumerable.SequenceEqual(address, bx.miner))
                {
                    start += Environment.InitialCoinPerBlock / ((bx.block_number / Environment.diffReducer) + 1);
                }
                
                // Add and subtract coins gained and lost through transactions
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


        /// <summary>
        /// Get the last hash in the blockchain.
        /// </summary>
        /// <returns>The last hash in the block chain.</returns>
        public byte[] getLastHash() => chain[chain.Count - 1].hash;


        /// <summary>
        /// Add a new block to the chain.
        /// </summary>
        /// <param name="b"></param>
        public void AddBlock(Block b)
        {
            chain.Add(b);
        }


        /// <summary>
        /// Default constructor for newtonsoft.Json or oto create a brand new blockchain.
        /// </summary>
        public Blockchain() { }

    }
}
