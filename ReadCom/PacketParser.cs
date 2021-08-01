using System;
using System.Text;
using System.Text.RegularExpressions;
using ReadCom.Packets;
using ReadCom.Packets.Incoming;

namespace ReadCom
{
    public class PacketParser
    {
        static Regex dateRegex = new Regex(@"^(?'hour'[0-2]?[0-9]):(?'min'[0-5]?[0-9]):(?'sec'[0-5]?[0-9]) (?'day'[1-9]|([012][0-9])|(3[01]))-(?'month'[0]{0,1}[1-9]|1[012])-(?'year'\d\d\d\d) \((?'epoc'[0-9]*)\)", RegexOptions.Compiled);
        public static Packet parsePacket(byte[] packetBytes, int packetId = 0)
        {
            
            Packet packet = null;
            string str = Encoding.ASCII.GetString(packetBytes);
            if (str.StartsWith("Connected,"))
            {
                packet = new ConnectedPacket(packetBytes, packetId);
            }
            else if (str.StartsWith("V="))
            {
                packet = new VoltagePacket(packetBytes, packetId);
            }
            else if (str.StartsWith("0,"))
            {
                packet = new ChipTimePacket(packetBytes, packetId);
            }
            else if (dateRegex.IsMatch(str))
            {
                packet = new GetTimeResponsePacket(packetBytes, packetId);
            }
            else
            {
                packet = new UnknownPacket(packetBytes, packetId);
            }

            return packet;
        }
    }
}