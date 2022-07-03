using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ReadCom.Models;
using ReadCom.Packets;

namespace ReadCom
{
    public class TCP
    {
        private readonly Client _client;

        public TCP(Client client)
        {
            _client = client;
        }

        private TcpClient _socket;

        private NetworkStream _stream;
        private Packet _receivedData;
        private byte[] _receiveBuffer;

        public bool IsConnected => !(_socket is null) && _socket.Connected;

        /// <summary>Attempts to connect to the server via TCP.</summary>
        public void Connect()
        {
            _socket = new TcpClient
            {
                ReceiveBufferSize = Client.DataBufferSize,
                SendBufferSize = Client.DataBufferSize
            };

            _receiveBuffer = new byte[Client.DataBufferSize];

            //_socket.BeginConnect(_client.Ip, _client.Port, ConnectCallback, _socket);


            var result = _socket.BeginConnect(_client.Ip, _client.Port, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (!success)
            {
                throw new Exception("Failed to connect.");
            }

            try
            {
                _socket.EndConnect(result);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

            if (!_socket.Connected)
            {
                return;
            }

            _stream = _socket.GetStream();

            _receivedData = new Packet();

            _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        }

        ///// <summary>Initializes the newly connected client's TCP-related info.</summary>
        //private void ConnectCallback(IAsyncResult result)
        //{
        //    try
        //    {
        //        _socket.EndConnect(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(ex.Message);
        //    }

        //    if (!_socket.Connected)
        //    {
        //        return;
        //    }

        //    _stream = _socket.GetStream();

        //    _receivedData = new Packet();

        //    _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
        //}

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="packet">The packet to send.</param>
        public void SendData(Packet packet)
        {
            try
            {
                if (_socket != null)
                {
                    _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to server
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error sending data to server via TCP: {ex}");
            }
        }

        private static int packetId = 0;

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = _stream.EndRead(result);
                if (byteLength <= 0)
                {
                    _client.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(_receiveBuffer, data, byteLength);

                LogHelper.Trace("received:" + Encoding.ASCII.GetString(data));
                packetId++;

                _receivedData.Reset(HandleData(data, packetId)); // Reset receivedData if all data was handled
                _stream.BeginRead(_receiveBuffer, 0, Client.DataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                _client.Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="data">The recieved data.</param>
        private bool HandleData(byte[] data, int pId)
        {
            int packetLength = 0;

            _receivedData.SetBytes(data);
            if (_receivedData.UnreadLength() >= 0)
            {
                // If client's received data contains a packet
                string str = _receivedData.ReadString(false);
                packetLength = str.IndexOf('\n') + 1;
                if (packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] packetBytes = _receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = PacketParser.parsePacket(packetBytes, pId))
                    {
                        if (_client.PacketHandlers.ContainsKey(packet.PacketType))
                        {
                            _client.PacketHandlers
                                [packet.PacketType](packet); // Call appropriate method to handle the packet
                        }
                    }
                });
                packetLength = 0; // Reset packet length
                if (_receivedData.UnreadLength() >= 0)
                {
                    // If client's received data contains another packet
                    string str = _receivedData.ReadString(false);
                    packetLength = str.IndexOf('\n') + 1;
                    if (packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }

        /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            _socket.Close();
            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            _socket = null;
        }
    }
}