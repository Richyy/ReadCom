using System;

namespace ReadCom.Models
{
    public class Reader
    {
        public int Id { get; set; }
        public int Ip { get; set; }
        public string TimingPoint { get; set; }
        public int ConnectionStatus { get; set; }
        public int ReadingStatus { get; set; }
    }
}