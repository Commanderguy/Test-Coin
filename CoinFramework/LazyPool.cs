using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Coin
{
    /// <summary>
    /// The lazypool contains transactions that were not yet added to the most up-to-date blockchain. Priority should ideally given to the most old transactions, but thats an implementation detail. 
    /// Also sharing the own LazyPool is an act of politeness (and also helps everyone who made an transaction).
    /// </summary>
    public class LazyPool
    {
        Stack<Transaction> st = new Stack<Transaction>();

        public LazyPool(Transaction tx)
        {
            st.Push(tx);
        }

        public LazyPool() { }

        public Block ForgeBlock()
        {
            List<Transaction> ret = new List<Transaction>();
            for(int i = 0; i < Environment.MaxTransactionsPerBlock; i++)
            {
                if (st.Count == 0) break;
                ret.Add((Transaction)st.Pop());
            }
            Block b = new Block(ret);
            return b;
            
        }

    }
}
