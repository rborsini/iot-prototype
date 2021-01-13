using Microsoft.Azure.Devices.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Azure_SimulatedDevice
{
    class Program
    {

        private static string connectionString = "HostName=rob-iothub.azure-devices.net;DeviceId=device1;SharedAccessKey=vRMaS9Ds66Gb54XYfzn9f5BeW9DM/rfE4vjUZOJ5Ock=";

        static async Task Main(string[] args)
        {

            var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            // invio messaggio
            await SendDeviceToCloudMessagesAsync(deviceClient, cts.Token);

            deviceClient.Dispose();
            Console.WriteLine("Device simulator finished.");
        }

        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync(DeviceClient deviceClient, CancellationToken ct)
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            var rand = new Random();

            while (!ct.IsCancellationRequested)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                string messageBody = JsonSerializer.Serialize(new { temperature = currentTemperature, humidity = currentHumidity });
                using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                    CreationTimeUtc = DateTime.Now                    
                };

                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageBody}");

                await Task.Delay(1000);
            }
        }

    }
}
