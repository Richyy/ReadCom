namespace ReadCom.Packets.Incoming
{
    public class ConnectedPacket : Packet
    {
        public ConnectedPacket(byte[] _data, int packetId=0) : base(_data, packetId)
        {
            _packetType = (int) ServerPacketTypes.connected;
        }

        public void teszt()
        {
            
        }
    }
}