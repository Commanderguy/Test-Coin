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

        NodeDB _db;
        Thread th;
        public Node(Blockchain ch, NodeDB db)
        {
            Listener.Start();
            th = new Thread(LoopListen);
            th.Start();
            _chain = ch;
            _db = db;
        }

        public bool stop = false;

        Blockchain _chain;

        public void stopThread()
        {
            th.Abort();
        }

        private void LoopListen()
        {
            byte[] buf = new byte[256];
            while (!stop)
            {
                // Listen to tcp clients and process them.
                TcpClient client = Listener.AcceptTcpClient();
                var stream = client.GetStream();

                var i = stream.Read(buf, 0, buf.Length);
                string msg = ASCIIEncoding.ASCII.GetString(buf, 0, i);

                if (msg.Contains("COINP2PNODEEXCHANGE;"))
                { 
                    var answer = ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(_db));
                    var ip = msg.Replace("COINP2PNODEEXCHANGE;", "");
                    _db.processNewNode(ip);
                    stream.Write(answer, 0, answer.Length);
                }else if(msg.Contains("COINP2PCHAIN;"))
                {
                    msg = msg.Replace("COINP2PCHAIN;", "");
                    if(Convert.ToInt32(msg) >= _chain.chain.Count)
                    {
                        Console.WriteLine("Clients size is equal or bigger");
                        var WriteBuf = ASCIIEncoding.ASCII.GetBytes("ENDOFCHAIN");
                        stream.Write(WriteBuf, 0, WriteBuf.Length);
                    }
                    else
                    {
                        for(int b = Convert.ToInt32(msg); b < _chain.chain.Count; b++)
                        {
                            var writebuf = ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(_chain.chain[b]));
                            stream.Write(writebuf, 0, writebuf.Length);
                        }
                        var WriteBuf = ASCIIEncoding.ASCII.GetBytes("ENDOFCHAIN");
                        stream.Write(WriteBuf, 0, WriteBuf.Length);
                    }
                }else if(msg.Contains("NEWTRANSACTION"))
                {
                    _db.pool.addTx(Newtonsoft.Json.JsonConvert.DeserializeObject<Transaction>(msg));
                }

                client.Close();
            }
        }


        ~Node()
        {
            Listener.Stop();
        }
    }
}
