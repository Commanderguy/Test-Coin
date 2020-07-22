using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Coin;

namespace BlockChainNode
{
    class MissingBlockPart
    {
        public List<Block> blocks { get; }
        public void AddBlock(Block b) => blocks.Add(b);
        public MissingBlockPart()
        {
            blocks = new List<Block>();
        }
    }
}
