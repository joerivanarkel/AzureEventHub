[← Return to AZ-204](https://github.com/joerivanarkel/joerivanarkel/blob/main/AZ204.md)<br>

[![.NET](https://github.com/joerivanarkel/AzureEventHub/actions/workflows/dotnet.yml/badge.svg)](https://github.com/joerivanarkel/AzureEventHub/actions/workflows/dotnet.yml)

# Azure Event Hub
In this example I am Sending and Receiving messages with Azure Event Hub. For this I used the: `Azure.Messaging.EventHubs`, `Azure.Messaging.EventHubs.Processor`, `Azure.Storage.Blobs` NuGet packages. I used Blob storage with the `EventProccesoorClient`, which will come up later on. I also used dotnet secrets with my `joerivanarkel.UserSecrets` package.

Before writing any code i have created am Azure Event Hub namespace and a Storage Account in the Azure Portal. Furthermore i created an Event Hub in the Namespace. In this example is how to work with the Event Hub.

## Sending Data to an Event Hub
I firstly create a Batch of events I will be sending to the Event Hub, with the `EventDataBatch` object. I used the `EventHubProducerClient` i created earlier. Then i add 5 events to this batch. I send these to the Event Hub with the `SendAsync()` method. Finally i close the connection.

```csharp
using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

for (int i = 1; i <= 5; i++)
{
  var result = eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}")));
  if (!result)
  {
    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
  }
}

await producerClient.SendAsync(eventBatch);
Console.WriteLine($"A batch of 5 events has been published.");
await producerClient.DisposeAsync();
return true;

```

## Recieving Data from an Event Hub
Using the `EventProcessorClient` class, I assign the Event and Error events to the methods which will handle them. Then I start the processor, wait and close the connection. This method on maintains a connection and sends the events to the right handler.

```csharp
processor.ProcessEventAsync += ProcessEventHandler;
processor.ProcessErrorAsync += ProcessErrorHandler;

await processor.StartProcessingAsync();
await Task.Delay(TimeSpan.FromSeconds(30));
await processor.StopProcessingAsync();

Task processEventHandler(ProcessEventArgs eventArgs) => Task.CompletedTask;
Task processErrorHandler(ProcessErrorEventArgs eventArgs) => Task.CompletedTask;
return true;
```

In these handlers I take the arguments given and display the received or return the exception.

```csharp
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
```
