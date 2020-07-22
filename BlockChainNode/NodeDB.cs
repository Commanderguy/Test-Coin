using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Test_Coin;

namespace BlockChainNode
{
    
    public class NodeDB
    {
        List<string> KnownNodes = new List<string>();

        Stack<string> shuffeled_nodes = new Stack<string>();
        Random rand;
        
        public NodeDB(string nodeFile)
        {

            if (File.Exists(nodeFile))
                KnownNodes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(nodeFile);
            else
                throw new FileNotFoundException("Node db not found", nodeFile);
            rand = new Random(DateTime.Now.Millisecond * KnownNodes.Count);
            shuffeled_nodes = new Stack<string>( KnownNodes.OrderBy(x => rand.Next()).ToList());
        }

        private Blockchain _chain;

        /// <summary>
        /// Lookup all nodes in the database
        /// </summary>
        public void Lookup(Test_Coin.Blockchain chain)
        {
            _chain = chain;
            for(; ; )
            {
                while(shuffeled_nodes.Count != 0)
                {
                    connect_to_node(shuffeled_nodes.Pop());
                }
                shuffeled_nodes = new Stack<string>(KnownNodes.OrderBy(x => rand.Next()).ToList());
            }
        }


        private void connect_to_node(string hostname)
        {
            TcpClient client = new TcpClient(hostname, 8080);
            var stream = client.GetStream();
            byte[] greeting = ASCIIEncoding.ASCII.GetBytes("NODE EXCHANGE v1;ChainSize: " + _chain.chain.Count.ToString() + ";NodeAdress: " + Node.GetLoacalIp().ToString());
            stream.Write( greeting, 0, greeting.Length );
            
        }


        /// <summary>
        /// Process a new node and return if it is already contained in the database.
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        private bool processNewNode(string hostname)
        {
            if (KnownNodes.Contains(hostname)) return false;
            if (hostname == Node.GetLoacalIp().ToString()) return false;
            KnownNodes.Add(hostname);
            shuffeled_nodes.Push(hostname);
            return true;
        }

    }
}
