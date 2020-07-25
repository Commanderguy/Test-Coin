using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
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

            Block bl = new Block();

            AccountView view = new AccountView();

            Block nullBlock = new Block();
            nullBlock.calculateNonce(view.publicKey);
            //chain.AddBlock(nullBlock);
            


            Console.WriteLine("Your public key: " + ASCIIEncoding.ASCII.GetString( view.publicKey));


            LazyPool pool = new LazyPool();

            AccountView view2 = new AccountView();

            Transaction tx = new Transaction(view.publicKey, view2.publicKey, 30, 0, view.privateKey);
            pool.addTx(tx);
            Block b2 = pool.ForgeBlock();
            b2.block_number = chain.chain.Count;
            b2.calculateNonce(view.publicKey);
            b2.prev_hash = chain.chain[chain.chain.Count - 1].hash;
            b2.calculateNonce(view.publicKey);

            chain.AddBlock(b2);

            Console.WriteLine("View1 balance: " + chain.count_funds(view.publicKey) + "\nView2 balance: " + chain.count_funds(view2.publicKey));

            Console.WriteLine("Stats after transaction");








            Console.WriteLine("Is chain valid: " + chain.validate());


            onFinalizer();
            Console.ReadKey();
        }

        static void onFinalizer()
        {
            Console.WriteLine("Writing text to file");
            File.WriteAllText("tst.chain", JsonConvert.SerializeObject(chain));
            Console.WriteLine("Saved chain to file");
        }


    }
}
