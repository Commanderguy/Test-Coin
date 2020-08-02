using CoinFramework;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

namespace ChainSync
{
    public class ChainSyncer
    {
        Blockchain _chain;

        public ChainSyncer(Blockchain chain)
        {
            _chain = chain;
            Task.Run(() =>
            {
                NetworkComms.AppendGlobalIncomingPacketHandler<int>("SendChain", AcceptSend);
                NetworkComms.AppendGlobalIncomingPacketHandler<PeerList>("SendPeers", AcceptPeers);
            });
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

        public void AcceptPeers(PacketHeader header, Connection connection, PeerList newPeers)
        {

        }
        
    }
}
