using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync("task-queue", durable: true, exclusive: false, autoDelete: false);

Console.WriteLine("[*] Waiting for messages.... To exit press CTRL+C");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[X] Received: {message}");

    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000); // Simulate work by delaying based on the number of dots in the message
    Console.WriteLine("[X] Done");

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(
    queue: "task-queue",
    autoAck: false, // Set to false to manually acknowledge messages,
    consumer: consumer
);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
