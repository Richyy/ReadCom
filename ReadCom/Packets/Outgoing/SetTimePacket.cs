using System;

namespace ReadCom.Packets.Outgoing
{
    public class SetTimePacket : Packet
    {
        public SetTimePacket(DateTime time)
        {
            _packetType = (int) ClientPackets.getTime;
            
            Write($"t {time:HH:mm:ss dd-MM-yyyy}");
        }
    }
}