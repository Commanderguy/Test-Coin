using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test_Coin;

namespace BlockChainNode
{
    public class Node
    {
        /// <summary>
        /// Retrieve the IP of the user.
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLoacalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        TcpListener Listener = new TcpListener( GetLoacalIp(), 8080);


        public Node(Blockchain ch)
        {
            Listener.Start();
            Thread th = new Thread(LoopListen);
            th.Start();
            _chain = ch;
        }

        public bool stop = false;

        Blockchain _chain;

        private void LoopListen()
        {
            byte[] buf = new byte[256];
            while (!stop)
            {
                // Listen to tcp clients and process them.
                TcpClient client = Listener.AcceptTcpClient();
                var stream = client.GetStream();

                int i = stream.Read(buf, 0, buf.Length);
                if (i == 0)
                {
                    client.Close();
                    continue;
                }
                string msgClient = ASCIIEncoding.ASCII.GetString(buf, 0, i);
                if(!msgClient.Contains("NODE EXCHANGE v1; ChainSize: ") && !msgClient.Contains(";NodeAdress:"))
                {
                    var n = ASCIIEncoding.ASCII.GetBytes("WRONG CHAIN FORMAT;STOPPING CONNECTION");
                    stream.Write(n, 0, n.Length );
                    client.Close();
                    continue;
                }

                int ChainSize = Convert.ToInt32( msgClient.Substring(29, msgClient.IndexOf(";NodeAdress")));
                if(ChainSize <= _chain.chain.Count )
                {
                    byte[] answer = SendMissingBlockchain(ChainSize);
                    stream.Write(answer, 0, answer.Length);
                }


                client.Close();
            }
        }

        private byte[] SendMissingBlockchain(int startpoint)
        {
            MissingBlockPart part = new MissingBlockPart();
            for(int i = startpoint; i < _chain.chain.Count; i++)
            {
                part.AddBlock(_chain.chain[i]);
            }
            return ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(part));
        }

        ~Node()
        {
            Listener.Stop();
        }
    }
}
