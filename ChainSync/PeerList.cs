using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoinFramework;
using System.CodeDom;

namespace ChainSync
{
    public class Peer
    {
        public string Ip { get; set; }
        public int Port { get; set; }

        public Peer() { }
        public Peer(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public static bool operator==(Peer one, Peer two)
        {
            return one.Ip == two.Ip && one.Port == two.Port;
        }


        public static bool operator!=(Peer one, Peer other)
        {
            return one.Ip != other.Ip && one.Port != other.Port;
        }

        public override bool Equals(object obj)
        {
            return obj is Peer peer &&
                   Ip == peer.Ip &&
                   Port == peer.Port;
        }

        public override int GetHashCode()
        {
            int hashCode = -14871390;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Ip);
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            return hashCode;
        }
    }


    public class PeerList
    {
        public List<Peer> Peers = new List<Peer>();
        public Peer own = new Peer(PeerHandler.GetLocalIPAddress(), CoinFramework.Environment.DefaultPort);
    }


    public class PeerHandler
    {
        public PeerList Peers = new PeerList();
        private string _folder;
        public PeerHandler(string folder)
        {
            _folder = folder;
            if (File.Exists(folder + ".peers"))
                Peers = JsonConvert.DeserializeObject<PeerList>(File.ReadAllText(folder + ".peers"));

        }
        private Random _rand = new Random();
        public Peer GetRandom()
        {
            return Peers.Peers.ElementAt(_rand.Next(0, Peers.Peers.Count));
        }

        public void AddList(PeerList list)
        {
            List<Peer> l = list.Peers;
            var res = from x in list.Peers where !Peers.Peers.Contains(x) && Peers.own != x select x;
            foreach(var n in res)
            {
                Peers.Peers.Add(n);
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetLocalIP6Address()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv6 address in the system!");
        }

        ~PeerHandler()
        {
            File.WriteAllText(_folder + ".peers", JsonConvert.SerializeObject(Peers));
        }
    }
}
