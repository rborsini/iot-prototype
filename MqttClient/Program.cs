using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MqttClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // local broker test
            //await LocalMqttClient();

            // azure broker test
            await AzureMqttClient();
        }

        /// <summary>
        /// Local connection example
        /// </summary>
        /// <returns></returns>
        private static async Task LocalMqttClient()
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

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("mqttdemo/setpoint")
                .WithPayload("1")
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            Console.ReadLine();
        }

        private static async Task AzureMqttClient()
        {
            var factory = new MqttFactory();

            var username = "rob-iothub.azure-devices.net/device1/api-version=2018-06-30";
            var password = "SharedAccessSignature sr=rob-iothub.azure-devices.net&sig=FiM7KkOa6i8S3vvUsS%2Bud7qwtbIndwBu%2FJ4Wk8GU%2FSM%3D&skn=iothubowner&se=1612833976";

            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("rob-iothub.azure-devices.net", 8883)
            .WithClientId("device1")
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            .WithCredentials(username, password)
            .WithTls()
            .WithCleanSession()
            .Build();

            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);
            Console.WriteLine("connected");

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("devices/device1/messages/events/$.ct=application%2Fjson&$.ce=utf-8")
                .WithPayload(JsonSerializer.Serialize(new { temperature = 11, humidity = 21 }))
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            Console.WriteLine("published");

            Console.ReadLine();
        }
    }
}
