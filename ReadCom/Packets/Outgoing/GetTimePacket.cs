namespace ReadCom.Packets.Outgoing
{
    public class GetTimePacket : Packet
    {
        public GetTimePacket()
        {
            _packetType = (int) ClientPackets.getTime;
            Write("r");
        }
    }
}