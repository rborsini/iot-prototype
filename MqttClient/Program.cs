using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Client1")
                .WithTcpServer("localhost")
                .Build();

            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();


            });

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("mqttdemo/setpoint").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);
            Console.WriteLine("connected");

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("mqttdemo/setpoint")
                .WithPayload("1")
                //.WithExactlyOnceQoS()docke
                //.WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            Console.WriteLine("published");

            Console.ReadLine();
        }
    }
}
