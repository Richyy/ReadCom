using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Text;
using Microsoft.Extensions.Logging;
using ReadCom.Packets;
using ReadCom.Models;
using System.Linq;

namespace ReadCom
{
    public class Client
    {
        public const int DataBufferSize = 4096;
        
        private ClientHandle clientEventHandler;

        public int readerId { get; set; }
        public int readerIp { get; set; }
        public string timingPoint { get; set; }

        public string Ip;
        public int Port;
        private TCP tcp;

        public DateTime ReaderTime { get; set; }

        private bool _isConnected = false;

        public bool IsConnected => _isConnected;
        public bool IsTCPConnected => tcp.IsConnected;

        public delegate void PacketHandler(Packet packet);

        public Dictionary<int, PacketHandler> PacketHandlers;

        public Client(string ip, int port)
        {
            Ip = ip;
            Port = port;
            clientEventHandler = new ClientHandle(this);
            
        }

        /// <summary>Attempts to connect to the server.</summary>
        public void ConnectToServer()
        {
            LogHelper.Trace("Reader"+readerIp+": Connecting to reader.");
            tcp = new TCP(this);
            InitializeClientData();

            _isConnected = true;
            tcp.Connect(); // Connect tcp, udp gets connected once tcp is done


            //using (var context = new ReaderContext())
            //{

            //    Reader reader = context.reader
            //                               .Where(r => r.id == readerId)
            //                               .FirstOrDefault();
            //    reader.connection_status = 3;
            //    context.SaveChanges();
            //}
        }

        /// <summary>Initializes all necessary client data.</summary>
        private void InitializeClientData()
        {
            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int) ServerPacketTypes.unknown, clientEventHandler.UnknownPacket},
                {(int) ServerPacketTypes.connected, clientEventHandler.Connected},
                {(int) ServerPacketTypes.voltage, clientEventHandler.VoltageStatus},
                {(int) ServerPacketTypes.chiptime, clientEventHandler.ChipTime},
                {(int) ServerPacketTypes.getTime, clientEventHandler.GetTime},
            };
            LogHelper.Trace("Reader"+readerIp+": Initialized packets.");
        }

        /// <summary>Disconnects from the server and stops all network traffic.</summary>
        public void Disconnect()
        {
            if (_isConnected)
            {
                _isConnected = false;
                tcp.Disconnect();

                //using(var context = new ReaderContext()) {

                //    Reader reader = context.reader
                //                               .Where(r => r.id == readerId)
                //                               .FirstOrDefault();
                //    reader.connection_status = 0;

                //    context.SaveChanges();
                //}

                LogHelper.Trace("Reader"+readerIp+": Disconnected from reader.");
            }
        }
        
        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="packet">The packet to send.</param>
        public void SendData(Packet packet)
        {
            LogHelper.Trace("Reader"+readerIp+": Sending "+ packet.GetType().Name +" to reader");
            tcp.SendData(packet);
        }
    }
}