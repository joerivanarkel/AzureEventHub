using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;

namespace EventHub
{
    public class EventHubRepository
    {
        private EventHubProducerClient producerClient;
        private BlobContainerClient storageClient;
        private EventProcessorClient processor;
        private string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        public EventHubRepository(string eventHubConnectionString, string eventHubName, string blobStorageConnectionString, string blobContainerName)
        {
            producerClient = new EventHubProducerClient(eventHubConnectionString, eventHubName);
            storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);
            processor = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);
        }

        public async Task<bool> SendDataAsync()
        {
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= 5; i++)
            {
                var result = eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}")));
                if (!result)
                {
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }
            try
            {
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of 5 events has been published.");
                await producerClient.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RecieveDataAsync()
        {
            try
            {
                processor.ProcessEventAsync += ProcessEventHandler;
                processor.ProcessErrorAsync += ProcessErrorHandler;

                await processor.StartProcessingAsync();
                await Task.Delay(TimeSpan.FromSeconds(30));
                await processor.StopProcessingAsync();

                Task processEventHandler(ProcessEventArgs eventArgs) => Task.CompletedTask;
                Task processErrorHandler(ProcessErrorEventArgs eventArgs) => Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        protected static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            Console.WriteLine($"Received event: {Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray())}");
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        protected static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            Console.WriteLine($"Partition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}