
using System;

namespace CoinFramework
{
    public class Environment
    {
        public const long diffReducer = 50000;
        public const long initialDifficulty = 2;
        public const long InitialCoinPerBlock = 50;
        public const long MaxTransactionsPerBlock = 5000;
        public const long MaxMessageSize = 256;
        public const int TimeBetweenNodeConnections = 10000;
        public const int DefaultPort = 12975;
        public const int TimeBetweenDiscovery = 1000;
        public const int TrustBlocks = 5;
    }
}
