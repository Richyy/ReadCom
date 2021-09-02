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
        private static RabbitMQHandler _rabbitMqHandler;
        private static bool _running = false;
        static void Main(string[] args)
        {
            _running = true;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            
            _rabbitMqHandler = new RabbitMQHandler("amqp://readcom:readcom@readcom.local:5672/");
            _rabbitMqHandler.Start();
            _rabbitMqHandler.CommandReceived += c_CommandReceived;
                
            
            while(_running)
            {
                ThreadManager.UpdateMain();
                Thread.Sleep(1);
            }
            
            _rabbitMqHandler.Stop();
            _rabbitMqHandler.Dispose();
        }
        
        static void c_CommandReceived(object sender, CommandEventArgs e)
        {
            
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _running = false;
            
            foreach (var client in _clients)
            {
                client.Disconnect();
            }
            
            _rabbitMqHandler.Stop();
            _rabbitMqHandler.Dispose();
        }
    }
}