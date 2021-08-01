using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReadCom
{
    class Program
    {
        private static List<Client> _clients = new List<Client>();
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Client cl = new Client("192.168.0.115",23);
            _clients.Add(cl);
            cl.ConnectToServer();
            while (cl.IsConnected)
            {
                ThreadManager.UpdateMain();
            }
        }
        
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var client in _clients)
            {
                client.Disconnect();
            }
        }
    }
}