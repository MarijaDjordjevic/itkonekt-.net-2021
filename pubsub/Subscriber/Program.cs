using System;
using EasyNetQ;
using Messages;

namespace Subscriber
{
    class Program
    {
        const string AMQP = "amqps://gsfihevq:eFtfoAM2d-JvSPxFBMu8te_VYbO91cBN@bunny.rmq.cloudamqp.com/gsfihevq";
            
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            using var bus = RabbitHutch.CreateBus(AMQP);
            //var subscriberId = "itkonekt";
            var subscriberId = "subscriber@marija";
            bus.PubSub.Subscribe<string>(subscriberId, message => {
                Console.WriteLine(message);
            });
            bus.PubSub.Subscribe<Greeting>(subscriberId, HandleGreeting);
            Console.WriteLine("Subscribed! Listening for messages!");
            Console.ReadLine();
        }

        static void HandleGreeting(Greeting g) {
            Console.WriteLine($"{g.From} says (in {g.Language}:");
            Console.WriteLine($"  {g.Text}");
        }
    }
}
