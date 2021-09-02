using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReadCom
{
    public class RabbitMQHandler : IDisposable
    {
        private ConnectionFactory factory;
        private IConnection conn;
        private IModel channel;
        private string consumerTag;

        public event EventHandler<CommandEventArgs> CommandReceived;

        public RabbitMQHandler(string uri)
        {
            factory = new ConnectionFactory();
            factory.Uri = new Uri(uri);
            factory.AutomaticRecoveryEnabled = true;
            factory.ClientProvidedName = "readcom rpi consumer";
            factory.DispatchConsumersAsync = true;
        }

        protected virtual void OnCommandReceived(CommandEventArgs e)
        {
            EventHandler<CommandEventArgs> handler = CommandReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Start()
        {
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare("amq.direct", ExchangeType.Direct, true);
            channel.QueueDeclare("command_queue", true, false, false, null);
            channel.QueueBind("command_queue", "amq.direct", "command_queue", null);
            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                var body = Encoding.UTF8.GetString(@ea.Body.ToArray());
                Command command = 
                    JsonSerializer.Deserialize<Command>(body);
                
                OnCommandReceived(new CommandEventArgs(command));
                
                lock (this.channel)
                {
                    this.channel.BasicAck(ea.DeliveryTag, false);
                }
                await Task.Yield();
            };
            consumerTag = channel.BasicConsume("command_queue", false, consumer);
        }

        public void Stop()
        {
            if (!(channel is null) && channel.IsOpen)
            {
                if (consumerTag.Length > 0)
                {
                    channel.BasicCancel(consumerTag);
                }

                channel.Close();
            }

            if (!(conn is null) && conn.IsOpen)
            {
                conn.Close();
            }
        }

        public void Dispose()
        {
            consumerTag = null;
            channel?.Dispose();
            channel = null;
            conn?.Dispose();
            conn = null;
        }
    }
}