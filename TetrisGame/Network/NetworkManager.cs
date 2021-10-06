using System;
using System.Text;
using System.Threading.Tasks;
using ENetOnline;

namespace MultiplayerTetris.Network
{
    public class NetworkManager
    {
        private Server server;

        private Client client;

        private ClientState clientState;
        
        public NetworkManager()
        {
            
        }

        public void CreateServer(ushort port, int playersLimit)
        {
            server = new Server(port, playersLimit);

            server.ClientConnected += ClientConnectedCallback;
            server.ClientDisconnected += ClientDisconnectedCallback;
            server.ClientTimedOut += ClientTimedOutCallback;
            server.PacketReceived += PacketReceivedCallback;
            
            server.StartListening();
        }

        public void SendTest()
        {
            byte[] nameRequestPacketBytes = new PlayerNameRequestPacket().ToBytes();
            server.Send(ref nameRequestPacketBytes, clientState);
        }

        private void ClientConnectedCallback(object sender, ClientEventArgs e)
        {
            Console.WriteLine(e.Client.IpAddress + " - connected");
            clientState = e.Client;
            SendTest();
        }
        
        private void ClientDisconnectedCallback(object sender, ClientEventArgs e)
        {
            Console.WriteLine(e.Client.IpAddress + " - disconnected");
        }
        
        private void ClientTimedOutCallback(object sender, ClientEventArgs e)
        {
            Console.WriteLine(e.Client.IpAddress + " - timedout");
        }
        
        private void PacketReceivedCallback(object sender, PacketEventArgs e)
        {
            GamePacket gamePacket = GamePacket.FromBytes(e.Payload);

            //Console.WriteLine("received packet" + Encoding.UTF8.GetString(e.Payload));
            switch (gamePacket)
            {
                case PlayerNameAnswerPacket p:
                    Console.WriteLine(p.PlayerName + " joined");
                    break;
            }
            
        }

        public void CreateClient()
        {
            client = new Client();
        }
        
        public void Connect(string hostOrIp, ushort port)
        {
            client.ConnectAsync(hostOrIp, port);
            client.StartListening();
            
            client.PacketReceived += ClientOnPacketReceived;
        }

        private void ClientOnPacketReceived(object sender, PacketEventArgs e)
        {
            GamePacket gamePacket = GamePacket.FromBytes(e.Payload);

            Console.WriteLine("received packet" + Encoding.UTF8.GetString(e.Payload));
            switch (gamePacket)
            {
                case PlayerNameRequestPacket p:
                    byte[] packet = new PlayerNameAnswerPacket("éâîo$-à%ù").ToBytes();
                    client.Send(ref packet);
                    break;
            }
        }
    }
}