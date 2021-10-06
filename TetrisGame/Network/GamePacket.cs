using System;

namespace MultiplayerTetris.Network
{
    public abstract partial class GamePacket
    {
        public PacketType Type { get; private set; }
        
        public int Size { get; private set; }

        public virtual byte[] ToBytes()
        {
            byte[] packet = new byte[Size];
            packet[0] = (byte)Type;
            return packet;
        }

        public abstract GamePacket FromBytes(ref byte[] packet);

        public GamePacket(PacketType packetType, int packetSize)
        {
            Type = packetType;
            Size = packetSize;
        }
    }
}