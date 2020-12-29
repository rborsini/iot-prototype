using MQTTnet;
using MQTTnet.Server;
using System;

namespace MqttServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(new MqttServerOptions());
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            mqttServer.StopAsync();

        }
    }
}
