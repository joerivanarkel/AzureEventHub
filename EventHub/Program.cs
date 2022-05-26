using EventHub;
using UserSecrets;

Console.WriteLine("Hello, World!");

string eventHubConnectionString = UserSecrets<Program>.GetSecret("eventHubConnectionString");
string eventHubName = "eventhubtest";

string blobStorageConnectionString = UserSecrets<Program>.GetSecret("blobStorageConnectionString");
string blobContainerName = "eventhubtest";

EventHubRepository eventHubRepository = new EventHubRepository(eventHubConnectionString, eventHubName, blobStorageConnectionString, blobContainerName);

await eventHubRepository.SendDataAsync();
await eventHubRepository.RecieveDataAsync();

