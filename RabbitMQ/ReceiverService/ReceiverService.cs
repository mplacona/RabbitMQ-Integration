using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiverService
{
    class ReceiverService : ServiceBase
    {
        private const string QueueName = "Q-Adwords-CreateCampaign";
        private Thread _thread;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);

        public ReceiverService()
        {
            ServiceName = "Q-Adwords-CreateCampaign Receiver";
        }

        public static void Main()
        {
            Run(new ReceiverService());
        }
        
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            _thread = new Thread(WorkerThreadFunc);
            _thread.Name = "Receiver Service Worker Thread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        private static void WorkerThreadFunc()
        {
            var factory = new ConnectionFactory { HostName = "127.0.0.1" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                // mark all messages as persistent
                const bool durable = true;
                channel.QueueDeclare(QueueName, durable, false, false, null);

                var consumer = new QueueingBasicConsumer(channel);

                // turn auto acknowledge off so we can do it manually
                const bool autoAck = false;
                channel.BasicConsume(QueueName, autoAck, consumer);

                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    byte[] body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(body);

                    Thread.Sleep(5000);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
        protected override void OnStop()
        {
            base.OnStop();
            _shutdownEvent.Set();
            if (!_thread.Join(3000))
            { // give the thread 3 seconds to stop
                _thread.Abort();
            }
        }
    }
}
