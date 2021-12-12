using System;
using EasyNetQ;
using Messages;

namespace Publisher
{
    class Program
    {
         const string AMQP = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";
            
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var bus = RabbitHutch.CreateBus(AMQP);
            var count = 1;
            while(true)
            {
                /*
                Console.WriteLine("Press a key to publish a message...");
                Console.ReadKey(false);
                var message = $"Message {count++} from {Environment.MachineName} at {DateTime.UtcNow:O}";
                bus.PubSub.Publish(message);
                Console.WriteLine($"Published message: {message}");
                */

                Console.WriteLine("Press 1 to publish a string, or 2 to publish a message");
                var key = Console.ReadKey(false);
                var message = $"Greeting {count++} from {Environment.MachineName} at {DateTime.UtcNow:O}";
                switch (key.KeyChar) {
                    case '1':
                        bus.PubSub.Publish(message);
                        Console.WriteLine($"Published message: {message}");
                        break;
                    case '2':
                        var greeting = new Greeting {
                            From = "Marija",
                            Text = message,
                            Language = "English"
                        };
                        bus.PubSub.Publish(greeting);
                        Console.WriteLine($"Published a greeting!");
                        break;
                    default:
                        Console.WriteLine("That's not a valid message key");
                        break;
                }
            }
        }
    }
}
