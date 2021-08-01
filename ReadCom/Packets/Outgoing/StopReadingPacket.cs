namespace ReadCom.Packets.Outgoing
{
    public class StopReadingPacket : Packet
    {
        public StopReadingPacket()
        {
            _packetType = (int)ClientPackets.stopReading;
            Write("S");
        }
    }
}