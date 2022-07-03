using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using ReadCom.Models;
using ReadCom.Packets;
using ReadCom.Packets.Incoming;
using ReadCom.Packets.Outgoing;

namespace ReadCom
{
    public class ClientHandle
    {
        private readonly Client _client;

        public ClientHandle(Client client)
        {
            _client = client;
        }
        public void UnknownPacket(Packet _packet)
        {
            string _msg = _packet.ReadString();

            LogHelper.Trace($"[{_packet.CreatedAt:HH:mm:ss.fff}][{_packet.PacketId}] UnknownPacket: {_msg}");
        }

        public void Connected(Packet _packet)
        {
            ConnectedPacket p = (ConnectedPacket) _packet;
            string _msg = p.ReadString();

            LogHelper.Trace($"[{p.CreatedAt:HH:mm:ss.fff}][{p.PacketId}] Connected: {_msg}");
            _client.SendData(new GetTimePacket());
            
            //_client.SendData(new StartReadingPacket());
        }

        public void VoltageStatus(Packet _packet)
        {
            //string _msg = _packet.ReadString();

            //LogHelper.Trace($"[{_packet.CreatedAt:HH:mm:ss.fff}][{_packet.PacketId}] Voltage status message: {_msg}");
        }
        
        public void GetTime(Packet _p)
        {
            GetTimeResponsePacket packet = (GetTimeResponsePacket) _p;
            _client.ReaderTime = packet.ReaderTimer;

            var diff = Math.Abs((DateTime.Now - _client.ReaderTime).TotalSeconds);
            LogHelper.Trace($"Current reader time: {packet.ReaderTimer:yyyy-MM-dd HH:mm:ss} diff from current time in seconds: {diff}");
            if (diff > 10)
            {
                _client.SendData(new SetTimePacket(DateTime.Now));
            }
        }

        public void ChipTime(Packet _p)
        {
            ChipTimePacket packet = (ChipTimePacket) _p;
            
            //using (var db = new ChipTimeContext())
            //{
            //    var chipTime = new ChipTime()
            //    {
            //        AntennaNo = packet.AntennaNo,
            //        ChipCode = packet.ChipCode,
            //        ChipTimeDt = packet.ChipTime,
            //        ChipTimeRaw = packet.ChipTimeRaw,
            //        ChipTimeRawMillisec = packet.ChipTimeRawMillisec,
            //        IsRewind = packet.IsRewind,
            //        LogId = packet.LogId,
            //        ReaderNo = packet.ReaderNo,
            //        ReaderTime = packet.ReaderTime,
            //        Rssi = packet.Rssi,
            //        StartTime = packet.StartTime,
            //        UltraId = packet.UltraId,
            //    };
            //    db.ChipTimes.Add(chipTime);
            //    db.SaveChanges();
            //}

            LogHelper.Trace($"[{packet.CreatedAt:HH:mm:ss.fff}][{packet.PacketId}] Chiptime message: {packet.ChipTime:HH:mm:ss.fff} - {packet.ChipCode}");
        }
    }
}