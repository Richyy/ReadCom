using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ReadCom
{
    class Program
    {
        private static List<Client> _clients = new List<Client>();
        private static RabbitMQHandler _rabbitMqHandler;
        private static bool _running = false;
        private static IHost _host;
        static void Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();
            _host.RunAsync();
            _running = true;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            _rabbitMqHandler = new RabbitMQHandler("amqp://readcom:readcom@readcom.local:5672/");
            _rabbitMqHandler.Start();
            _rabbitMqHandler.CommandReceived += c_CommandReceived;


            while (_running) {
                ThreadManager.UpdateMain();
                Thread.Sleep(1);
            }

            _host.StopAsync().GetAwaiter().GetResult();
            
            _rabbitMqHandler.Stop();
            _rabbitMqHandler.Dispose();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseUrls("http://readcom.local:5000").UseStartup<Startup>(); });

        static void c_CommandReceived(object sender, CommandEventArgs e)
        {
            try
            {
                Command command = e.Command;
                Client selectedClient = null;
                foreach (Client client in _clients)
                {
                    if (client.readerId == command.data.readerId)
                    {
                        selectedClient = client;
                        break;
                    }
                }
                LogHelper.Trace("Command received: " + command.commandType);


                if (selectedClient is null)
                {
                    if (command.data.readerIp > 0)
                    {
                        selectedClient = new Client("192.168.0." + command.data.readerIp, 23);
                        selectedClient.readerId = command.data.readerId;
                        selectedClient.readerIp = command.data.readerIp;
                        selectedClient.timingPoint = command.data.timingPoint;
                        _clients.Add(selectedClient);
                    }
                    else
                    {
                        LogHelper.Error("There was no client and got no ip address");
                        return;
                    }
                }

                if (command.commandType == Command.COMMAND_TYPE_CONNECT)
                {
                    selectedClient.ConnectToServer();
                }
                else if (command.commandType == Command.COMMAND_TYPE_DISCONNECT)
                {
                    selectedClient.Disconnect();
                }
                else if (command.commandType == Command.COMMAND_TYPE_UPDATE)
                {
                    if (selectedClient.IsConnected && selectedClient.readerIp != command.data.readerIp)
                    {
                        selectedClient.readerIp = command.data.readerIp;
                        selectedClient.Ip = "192.168.0." + selectedClient.readerIp;
                        selectedClient.Disconnect();
                        selectedClient.ConnectToServer();
                    }

                    selectedClient.readerId = command.data.readerId;
                    selectedClient.readerIp = command.data.readerIp;
                    selectedClient.Ip = "192.168.0." + selectedClient.readerIp;
                    selectedClient.timingPoint = command.data.timingPoint;
                }
            } catch(Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _running = false;

            foreach (var client in _clients) {
                client.Disconnect();
            }

            _rabbitMqHandler.Stop();
            _rabbitMqHandler.Dispose();
        }
    }
}