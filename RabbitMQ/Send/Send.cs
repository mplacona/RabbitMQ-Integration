using System;
using RabbitMQ.Client;

namespace Send
{
    class Send
    {
        private const string QueueName = "QTransactions";
        private const int Iterations = 100000000;
        public static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("You must specify a message");
                return;
            }

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                // mark all messages as persistent
                const bool durable = true;
                channel.QueueDeclare(QueueName, durable, false, false, null);

                // Set delivery mode (1 = non Persistent | 2 = Persistent)
                IBasicProperties props = channel.CreateBasicProperties();
                props.DeliveryMode = 2;

                for (int i = 0; i < Iterations; i++)
                {
                    string msg = String.Format("{0} #{1}", args[0], i);
                    byte[] body = System.Text.Encoding.UTF8.GetBytes(msg);
                    channel.BasicPublish("", QueueName, props, body);
                    Console.WriteLine(" [x] Sent {0}", msg);
                }
            }
        }
    }
}
