using CoinFramework;

namespace ChainSync
{
    public class ChainSyncer
    {
        Blockchain _chain;

        public ChainSyncer(Blockchain chain)
        {
            _chain = chain;
        }

        public delegate void onConnected();
        public onConnected onConnection { get; set; }
        
    }
}
