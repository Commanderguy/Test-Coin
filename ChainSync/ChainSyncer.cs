using CoinFramework;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ChainSync
{

    class lookFor
    {
        PeerHandler _handler;
        Blockchain _chain;
        public lookFor(Blockchain chain, PeerHandler handler)
        {
            _handler = handler;
            _chain = chain;
            NetworkComms.AppendGlobalIncomingPacketHandler<Block>("AnswerChain", AcceptBlock);
        }

        int chainSize = 0;

        public void AcceptBlock(PacketHeader header, Connection connection, Block newBlock)
        {
            if (newBlock == null) { ConnectionHandled(); return; }
            if(!newBlock.isValid((newBlock.block_number / Environment.diffReducer) + Environment.initialDifficulty, _chain)) { ConnectionHandled(); return; }
            if(!Enumerable.SequenceEqual( newBlock.prev_hash, _chain.getLastHash()))
            {
                //If the other chain is x blocks longer, request last 10 blocks;
                if(chainSize++ >= CoinFramework.Environment.TrustBlocks)
                {
                    chainSize = 0;
                    connection.SendObject<int>("SendChain", _chain.chain.Count - 10);
                    ConnectionHandled();
                    return;
                }
                else
                {
                    return;
                }
            }
            if(newBlock.block_number != _chain.chain.Count + 1)
            {
                ConnectionHandled();
                return;
            }
            _chain.AddBlock(newBlock);
            connection.SendObject<int>("SendChain", _chain.chain.Count + 1);
        }

        public void ConnectionHandled() => working = false;
        

        bool gotChain = false;
        System.Timers.Timer t;
        bool working = true;
        public void GetResOrVoid()
        {
            
            t = new System.Timers.Timer(CoinFramework.Environment.TimeBetweenDiscovery);
            t.Elapsed += T_Elapsed;
            while(working)
            { }
            return;
        }

        public void Stop()
        {
            NetworkComms.CloseAllConnections();
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            working = false;
            t.Stop();
        }

        public void BackgroundRun()
        {
            Task.Run(() =>
            {
                foreach (var x in _handler.Peers.Peers)
                {
                    NetworkComms.SendObject<int>("SendChain", x.Ip, x.Port, _chain.chain.Count + 1);
                }
            });
        }

    }







    public class ChainSyncer
    {
        lookFor lookup;
        Blockchain _chain;

        public ChainSyncer(Blockchain chain, string Folder)
        {
            lookup = new lookFor(chain, handler);
            handler = new PeerHandler(Folder);
            _chain = chain;
            Task.Run(() =>
            {
                NetworkComms.AppendGlobalIncomingPacketHandler<int>("SendChain", AcceptSend);
                NetworkComms.AppendGlobalIncomingPacketHandler<PeerList>("SendPeers", AcceptPeers);
            });
            lookup.BackgroundRun();
        }

        public void AcceptSend(PacketHeader header, Connection connection, int msg)
        {
            if (!(_chain.chain.Count > msg))
            {
                connection.SendObject<Block>("AnswerChain", _chain.chain[msg]);
                return;
            }
            connection.SendObject<Block>("AnswerChain", null);
            connection.CloseConnection(false);
        }

        PeerHandler handler;

        public void AcceptPeers(PacketHeader header, Connection connection, PeerList newPeers)
        {
            handler.AddList(newPeers);
        }

        ~ChainSyncer()
        {
            lookup.Stop();
        }
        
    }
}
