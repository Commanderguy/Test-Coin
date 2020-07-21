using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test_Coin
{
    public class Program
    {
        public static Blockchain chain;
        static void Main(string[] args)
        {
            ECDsa dsa = ECDsa.Create();
            
            if (!File.Exists("tst.chain"))
            {
                chain = new Blockchain();
            }else
            {
                 chain = JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText("tst.chain"));
            }



            Block b = new Block();
            b.AddTransaction(new byte[] { 2, 0, 0, 4, 5, 4, 3, 2, 1 }, new byte[] { 4, 4, 4, 4, 2, 21 }, 50);
            Console.WriteLine("Created block");
            b.calculateNonce(new byte[] { 2, 0, 0, 4, 5, 4, 3, 2, 1 });
            chain.AddBlock(b);

            Console.WriteLine("Funds of receiving address: " + chain.count_funds(new byte[] { 4, 4, 4, 4, 2, 21 }));
            onFinalizer();
            Console.ReadKey();
        }

        static void onFinalizer()
        {
            File.WriteAllText("tst.chain", JsonConvert.SerializeObject(chain));
        }


    }
}
