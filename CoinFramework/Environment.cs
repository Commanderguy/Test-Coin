using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFramework
{
    public class Environment
    {
        public const long diffReducer = 50000;
        public const long initialDifficulty = 3;
        public const long InitialCoinPerBlock = 50;
        /// <summary>
        /// Internal size limit for blocks.
        /// </summary>
        public const long MaxTransactionsPerBlock = 5000;
        public const long MaxMessageSize = 256;
        public const int TimeBetweenNodeConnections = 10000;
    }
}
