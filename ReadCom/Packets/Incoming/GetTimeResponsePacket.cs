using System;
using System.Text.RegularExpressions;

namespace ReadCom.Packets.Incoming
{
    public class GetTimeResponsePacket : Packet
    {
        private DateTime _dateTime;

        public DateTime ReaderTimer => _dateTime;

        Regex dateRegex = new Regex(@"^(?'hour'[0-2]?[0-9]):(?'min'[0-5]?[0-9]):(?'sec'[0-5]?[0-9]) (?'day'[1-9]|([012][0-9])|(3[01]))-(?'month'[0]{0,1}[1-9]|1[012])-(?'year'\d\d\d\d) \((?'epoc'[0-9]*)\)", RegexOptions.Compiled);
        public GetTimeResponsePacket(byte[] _data, int packetId=0) : base(_data, packetId)
        {
            _packetType = (int) ServerPacketTypes.getTime;
            string data = this.ReadString();
            if (!dateRegex.IsMatch(data))
            {
                throw new PacketException("Malformed GetTimeResponsePacket");
            }

            var match = dateRegex.Match(data);
            _dateTime = new DateTime(
                int.Parse(match.Groups["year"].Value),
                int.Parse(match.Groups["month"].Value),
                int.Parse(match.Groups["day"].Value),
                int.Parse(match.Groups["hour"].Value),
                int.Parse(match.Groups["min"].Value),
                int.Parse(match.Groups["sec"].Value)
                );
        }
    }
}