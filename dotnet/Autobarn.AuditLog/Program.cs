using System;
using System.IO;
using Autobarn.Messages;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace Autobarn.Website
{
	public class Program
	{
        private static readonly IConfigurationRoot config = ReadConfiguration();
        static void Main(string[] args)
        {
            var amqp = config.GetConnectionString("AutobarnRabbitMQConnectionString");
            var bus = RabbitHutch.CreateBus(amqp);
            var subscriberId = $"autobarn.auditlog@marija";
            bus.PubSub.Subscribe<NewVehicleMessage>(subscriberId, HandlerNewVehicleMessage);
            Console.WriteLine("Running Autobarn.AuditLog, Listening for messages...");
            Console.ReadKey();
        }

        private static void HandlerNewVehicleMessage(NewVehicleMessage message)
        {
            var csv = $"{message.Registration},{message.Manufacturer},{message.ModelName},{message.Year},{message.Color},{message.ListedAt:O}\n";
            File.AppendAllText("vehicles.log", csv);
            Console.WriteLine(csv);
        }
        private static IConfigurationRoot ReadConfiguration()
        {
            var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}