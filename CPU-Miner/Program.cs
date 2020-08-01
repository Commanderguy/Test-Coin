using CoinFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Miner
{
    class FProgram
    {
        static FileSystemWatcher watcher;

        static void Main(string[] args)
        {
            watcher = new FileSystemWatcher(args[0]);
            watcher.EnableRaisingEvents = true;
            watcher.Changed += Watcher_Changed;
           
            Console.WriteLine("Starting to parse blockchain");
            dir = args[0];

            
            fSize = new FileInfo(dir + ".blockChain").Length;

            miner = JsonConvert.DeserializeObject<keySafe>(File.ReadAllText(dir + ".MINERACC"));

            chain = JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText(dir + ".blockChain"));

            Console.Clear();
            Console.WriteLine("SHA-512 JSONCHAIN CPU MINER V1\nUse command 'help' for help or 'mine' to mine current LazyPool");
            commands.Add("help", cmdHelp);
            commands.Add("mine", mine);
            commands.Add("idle", idle);
            try
            {
                for (; ; )
                {
                    Console.Write(">> ");
                    string query = Console.ReadLine();
                    commands[query]("");
                }
            }catch(Exception e)
            {
                Console.WriteLine("Exception thrown: " + e.Message + "\nPress enter to exit");
                Console.ReadLine();
            }
        }

        static long fSize;

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.Name == ".LazyPool")
                {
                    List<Transaction> txs;
                    if (File.Exists(dir + ".LazyPool"))
                    {
                        txs = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(dir + ".LazyPool"));
                        File.Delete(dir + ".LazyPool");
                    }
                    else
                        txs = new List<Transaction>();

                    foreach (var x in txs)
                    {
                        pool.addTx(x);
                    }
                }
            }catch(Exception)
            {

            }
        }

        static Blockchain chain;
        static string dir;


        public static keySafe miner = new keySafe();

        static Dictionary<string, command> commands = new Dictionary<string, command>();



        public static void cmdHelp(string query)
        {
            Console.WriteLine("HELP\nType mine to start mining with the .Lazypool file\nType 'idle' for repeated mining");
        }
        
        static LazyPool pool = new LazyPool();

        public static void mine(string query)
        {
            Console.WriteLine("--------------------------------------------------------------");
            try
            {
                if(new FileInfo(dir + ".blockChain").Length != fSize)
                {

                }
                bool dLog = query.Contains("-debug");
                
                List<Transaction> txs;
                if (File.Exists(dir + ".LazyPool"))
                {
                    txs = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(dir + ".LazyPool"));
                    File.Delete(dir + ".LazyPool");
                }
                else
                    txs = new List<Transaction>();

                foreach (var x in txs)
                {
                    pool.addTx(x);
                }

                Block nBlock = pool.ForgeBlock();
                nBlock.prev_hash = chain.getLastHash();
                nBlock.block_number = chain.chain.Count;
                nBlock.calculateNonce(miner.publicKey, chain);

                fSize = new FileInfo(dir + ".blockChain").Length;
                if (!chain.validate())
                {
                    Console.WriteLine("Chain is invalid");
                    return;
                }

                chain.AddBlock(nBlock);
                File.WriteAllText(dir + ".blockChain", JsonConvert.SerializeObject(chain));
                   
            }
            catch(Exception e)
            {
                Console.WriteLine("Catched exception in mine method: " + e.Message);
            }
        }


        static public void idle(string query)
        {
            for (; ; )
                mine(query);
        }



        public struct keySafe
        {
            public byte[] publicKey;
            public byte[] privateKey;
            public keySafe(byte[] pubKey, byte[] privKey)
            {
                publicKey = pubKey;
                privateKey = privKey;
            }
        }

        public delegate void command(string query);
    }
}
