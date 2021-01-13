using Azure.Messaging.EventHubs.Consumer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure_MessageListener
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var eventHubConnectionString = "Endpoint=sb://germanywestcentraldedns001.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=ylUiWOO2RbOlW9a0d7ca3d1xhyIfME9aGbmfyyJc2No=;EntityPath=iothub-ehub-rob-iothub-6814786-f24df07502";

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            await using var consumerClient = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, eventHubConnectionString);

            await ReceiveMessagesFromDeviceAsync(consumerClient, cts.Token);

            Console.WriteLine("Cloud message reader finished.");
        }

        private static async Task ReceiveMessagesFromDeviceAsync(EventHubConsumerClient consumerClient,CancellationToken ct)
        {
            Console.WriteLine("Listening for messages on all partitions.");

            try
            {
                await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync(ct))
                {
                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    Console.WriteLine($"\tMessage body: {data}");
                }
            }
            catch (TaskCanceledException)
            {
                // This is expected when the token is signaled; it should not be considered an
                // error in this scenario.
            }
        }



    }
}
