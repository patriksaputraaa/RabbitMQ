using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    Uri = new Uri("amqp://guest:guest@localhost:5672")
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync("task-queue", durable: true, exclusive: false, autoDelete: false);

var message = GetMessage(args);
static string GetMessage(string[] args)
{
    return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
}
var body = System.Text.Encoding.UTF8.GetBytes(message);

var properties = new BasicProperties
{
    Persistent = true, // Make the message persistent
};
await channel.BasicPublishAsync(
    exchange: String.Empty,
    routingKey: "task-queue",
    mandatory: true,
    basicProperties: properties,
    body: body
);

Console.WriteLine($"[X] Sent: {message}");
// Wait for user input before closing the application
Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();