using System;

namespace ReadCom.Models
{
    public class ReaderSetting
    {
        public int Id { get; set; }
        public int AntennaNo { get; set; }
        public string ChipCode { get; set; }
        public DateTime ChipTimeDt { get; set; }
        public long ChipTimeRaw { get; set; }
        public long ChipTimeRawMillisec { get; set; }
        public int IsRewind { get; set; }
        public int LogId { get; set; }
        public int ReaderNo { get; set; }
        public long ReaderTime { get; set; }
        public int Rssi { get; set; }
        public long StartTime { get; set; }
        public int UltraId { get; set; }
    }
}