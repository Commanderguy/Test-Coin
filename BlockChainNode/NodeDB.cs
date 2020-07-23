using Newtonsoft.Json;
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
                KnownNodes = new List<string>();
            rand = new Random(DateTime.Now.Millisecond * KnownNodes.Count);
            shuffeled_nodes = new Stack<string>( KnownNodes.OrderBy(x => rand.Next()).ToList());
            processNewNode("192.168.178.74");
        }

        private Blockchain _chain;

        /// <summary>
        /// Lookup all nodes in the database
        /// </summary>
        public void Lookup(object chain)
        {
            _chain = (Test_Coin.Blockchain)chain;
            for(; ; )
            {
                while(shuffeled_nodes.Count != 0)
                {
                    connect_to_node(shuffeled_nodes.Pop());
                }
                shuffeled_nodes = new Stack<string>(KnownNodes.OrderBy(x => rand.Next()).ToList());
            }
        }
        

        public void connect_to_node(string hostname)
        {
            TcpClient client = new TcpClient(hostname, 8080);
            if (getNodes(client.GetStream()))
                TryGetChain(client.GetStream());

        }


        public bool getNodes(NetworkStream stream)
        {
            try
            {
                byte[] Greeting = ASCIIEncoding.ASCII.GetBytes("COINP2PNODEEXCHANGE;" + Node.GetLoacalIp().ToString());
                stream.Write(Greeting, 0, Greeting.Length);
                byte[] gainedNodes = new byte[65536];
                var i = stream.Read(gainedNodes, 0, gainedNodes.Length);

                string Nodes = ASCIIEncoding.ASCII.GetString(gainedNodes, 0, i);
                NodeDB _GainedDb = JsonConvert.DeserializeObject<NodeDB>(Nodes);
            }catch(System.Exception e)
            {
                return false;
            }
            return true;
        }


        public void TryGetChain(NetworkStream stream)
        {
            byte[] Greeting = ASCIIEncoding.ASCII.GetBytes("COINP2PCHAIN;" + _chain.chain.Count);
            stream.Write(Greeting, 0, Greeting.Length);
            byte[] recBlock = new byte[65536];
            stream.Read(recBlock, 0, recBlock.Length);
            bool placebo = false;
            while(ASCIIEncoding.ASCII.GetString(recBlock) != "ENDOFCHAIN")
            {
                if (placebo) continue;
                Block b = JsonConvert.DeserializeObject<Block>(ASCIIEncoding.ASCII.GetString(recBlock));
                if (b.isValid(b.block_number))
                {
                    _chain.AddBlock(b);
                }else
                {
                    placebo = true;
                    
                }
            }
        }



        /// <summary>
        /// Process a new node and return if it is already contained in the database.
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public bool processNewNode(string hostname)
        {
            if (KnownNodes.Contains(hostname)) return false;
            if (hostname == Node.GetLoacalIp().ToString()) return false;
            KnownNodes.Add(hostname);
            shuffeled_nodes.Push(hostname);
            return true;
        }

    }
}
