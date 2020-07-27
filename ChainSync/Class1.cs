using CoinFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainSync
{
    public class ChainSyncer
    {
        Blockchain _chain;

        public ChainSyncer(Blockchain chain)
        {
            _chain = chain;
        }
    }
}
