using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlockChainNode;
using System.Threading;
using System.Diagnostics;
using CoinFramework;

namespace Test_Coin
{
    public class Program
    {
        public static Blockchain chain;
        static void Main(string[] args)
        {
            if (!File.Exists("tst.chain"))
            {
                chain = new Blockchain();
            }else
            {
                 chain = JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText("tst.chain"));
            }

            NodeDB db = new NodeDB("nodes.list");
            Node node = new BlockChainNode.Node(chain, db);

            Thread th = new Thread(db.Lookup);
            th.Start(chain);




            Block bl = new Block();

            AccountView view = new AccountView();

            Block nullBlock = new Block();
            nullBlock.block_number = 0;
            nullBlock.mined = false;
            nullBlock.miner = view.publicKey;
            nullBlock.prev_hash = new byte[] { 0, 0, 0, 0 };

            nullBlock.calculateNonce(view.publicKey);


            chain.AddBlock(nullBlock);

            Console.WriteLine("Is chain valid: " + chain.validate());


            onFinalizer();
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
            
            
            th.Abort();
            node.stopThread();
            
        }

        static void onFinalizer()
        {
            Console.WriteLine("Writing text to file");
            File.WriteAllText("tst.chain", JsonConvert.SerializeObject(chain));
            Console.WriteLine("Saved chain to file");
        }


    }
}
