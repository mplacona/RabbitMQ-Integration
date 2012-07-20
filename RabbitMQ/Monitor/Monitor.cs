using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Receive
{
    private const string QueueName = "hello";
    public static void Main()
    {
        ConnectionFactory factory = new ConnectionFactory();
        factory.HostName = "127.0.0.1";
        using (IConnection connection = factory.CreateConnection())
        using (IModel channel = connection.CreateModel())
        {
            channel.QueueDeclare(QueueName, false, false, false, null);

            QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);

            bool autoAck = true;
            channel.BasicConsume(QueueName, autoAck, consumer);

            System.Console.WriteLine(" [*] Message List\n" +
                                     "To exit press CTRL+C");
            while (true)
            {
                BasicDeliverEventArgs ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                byte[] body = ea.Body;
                string message = System.Text.Encoding.UTF8.GetString(body);
                System.Console.WriteLine(" [x] Received {0}", message);
            }
        }
    }
}