namespace ReadCom.Packets.Incoming
{
    public class UnknownPacket : Packet
    {
        public UnknownPacket(byte[] _data, int packetId=0) : base(_data, packetId)
        {
            _packetType = (int) ServerPacketTypes.unknown;
        }
    }
}