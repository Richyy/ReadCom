namespace ReadCom.Packets.Outgoing
{
    public class StartReadingPacket : Packet
    {
        public StartReadingPacket()
        {
            _packetType = (int)ClientPackets.startReading;
            Write("R");
        }
    }
}