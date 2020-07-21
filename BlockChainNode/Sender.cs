using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockChainNode
{
    public class Node
    {
        /// <summary>
        /// Retrieve the IP of the user.
        /// </summary>
        /// <returns></returns>
        static IPAddress GetLoacalIp()
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


        public Node()
        {
            Listener.Start();
            Thread th = new Thread(LoopListen);
            th.Start();
        }

        public bool stop = false;

        private void LoopListen()
        {
            byte[] buf = new byte[256];
            while (!stop)
            {
                // Listen to tcp clients and process them.
                TcpClient client = Listener.AcceptTcpClient();
                var stream = client.GetStream();
                
                int i = 0;
                while((i = stream.Read(buf, 0, buf.Length)) != 0)
                {

                }

            }
        }

        public void AddAction(string name, Func<string> function)
        {
            _FuncAssociates.Add(name, function);
        }


        Dictionary<string, Func<string>> _FuncAssociates = new Dictionary<string, Func<string>>();
    }
}
