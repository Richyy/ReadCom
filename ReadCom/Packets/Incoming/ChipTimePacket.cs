using System;

namespace ReadCom.Packets.Incoming
{
    public class ChipTimePacket : Packet
    {
        private int _antennaNo;
        private string _chipCode;
        private DateTime _chipTime;
        private long _chipTimeRaw;
        private long _chipTimeRawMillisec;
        private int _isRewind;
        private int _logID;
        private int _readerNo;
        private long _readerTime;
        private int _rssi;
        private long _startTime;
        private int _ultraID;

        private string data;

        public ChipTimePacket(byte[] _data, int packetId = 0) : base(_data, packetId)
        {
            _packetType = (int) ServerPacketTypes.chiptime;
            data = this.ReadString();
            string[] splits = data.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length != 12)
            {
                throw new PacketException("Malformed ChipTimePacket");
            }


            _chipCode = splits[1];
            _chipTimeRaw = long.Parse(splits[2]);
            _chipTimeRawMillisec = long.Parse(splits[3]);
            _chipTime = (new DateTime(1980,1,1)).AddSeconds(_chipTimeRaw).AddMilliseconds(_chipTimeRawMillisec);
            _antennaNo = int.Parse(splits[4]);
            _rssi = int.Parse(splits[5]);
            _isRewind = int.Parse(splits[6]);
            _readerNo = int.Parse(splits[7]);
            _ultraID = int.Parse(splits[8]);
            _readerTime = long.Parse(splits[9]);
            _startTime = long.Parse(splits[10]);
            _logID = int.Parse(splits[11]);
        }

        public string ChipCode => _chipCode;
        public DateTime ChipTime => _chipTime;
        public long ChipTimeRaw => _chipTimeRaw;
        public long ChipTimeRawMillisec => _chipTimeRawMillisec;
        public int AntennaNo => _antennaNo;
        public int Rssi => _rssi;
        public int IsRewind => _isRewind;
        public int ReaderNo => _readerNo;
        public int UltraId => _ultraID;
        public long ReaderTime => _readerTime;
        public long StartTime => _startTime;
        public int LogId => _logID;
    }
}