namespace ReadCom.Packets.Incoming
{
    public class VoltagePacket : Packet
    {
        public VoltagePacket(byte[] _data, int packetId=0) : base(_data, packetId)
        {
            _packetType = (int) ServerPacketTypes.voltage;
        }
    }
}