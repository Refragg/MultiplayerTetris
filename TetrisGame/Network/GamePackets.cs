using System;
using System.Text;

namespace MultiplayerTetris.Network
{
    public enum PacketType : byte
    {
        PlayerNameRequest,
        PlayerNameAnswer,
        GameStateUpdate,
        PositionUpdate
    }

    public partial class GamePacket
    {
        public static GamePacket FromBytes(byte[] packet)
        {
            switch ((PacketType)packet[0])
            {
                case PacketType.PlayerNameRequest:
                    
                    return new PlayerNameRequestPacket().FromBytes(ref packet);
                case PacketType.PlayerNameAnswer:
                    return new PlayerNameAnswerPacket().FromBytes(ref packet);
            }

            return null;
        }
    }

    public class PlayerNameRequestPacket : GamePacket
    {
        public PlayerNameRequestPacket() : base(PacketType.PlayerNameRequest, 1)
        {
        }

        public override byte[] ToBytes()
        {
            byte[] packet = base.ToBytes();
            return packet;
        }

        public override GamePacket FromBytes(ref byte[] packet)
        {
            return this;
        }
    }

    public class PlayerNameAnswerPacket : GamePacket
    {
        public const int NameStringSize = 40;
        
        public string PlayerName;

        public PlayerNameAnswerPacket(string playerName) : base(PacketType.PlayerNameAnswer, NameStringSize + 1)
        {
            PlayerName = playerName;
        }

        public PlayerNameAnswerPacket() : base(PacketType.PlayerNameAnswer, NameStringSize + 1)
        {
        }
        
        public override byte[] ToBytes()
        {
            byte[] packetSpan = base.ToBytes();
            Encoding.UTF8.GetBytes(PlayerName).CopyTo(packetSpan, 1);
            return packetSpan;
        }

        public override GamePacket FromBytes(ref byte[] packet)
        {
            PlayerName = Encoding.UTF8.GetString(packet, 1, NameStringSize);
            return this;
        }
    }
}