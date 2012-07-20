using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    class Receive
    {
        private const string QueueName = "QTransactions";
        public static void Main()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
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

                System.Console.WriteLine(" [*] Waiting for messages." +
                                         "To exit press CTRL+C");
                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    byte[] body = ea.Body;
                    string message = System.Text.Encoding.UTF8.GetString(body);
                    System.Console.WriteLine(" [x] Processing {0}", message);

                    // Add some time to simulate processing
                    //Thread.Sleep(5000);

                    // Acknowledge message received and processed
                    System.Console.WriteLine(" Processed ", message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }
    }
}