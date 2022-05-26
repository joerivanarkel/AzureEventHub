using Xunit;
using EventHub;
using UserSecrets;

namespace EventHub.Test;

public class EventHubRepositoryTest
{
    [Fact]
    public void ShouldSendDataAsync()
    {
        string eventHubConnectionString = UserSecrets<EventHubRepositoryTest>.GetSecret("eventHubConnectionString");
        string eventHubName = "eventhubtest";

        string blobStorageConnectionString = UserSecrets<EventHubRepositoryTest>.GetSecret("blobStorageConnectionString");
        string blobContainerName = "eventhubtest";

        EventHubRepository eventHubRepository = new EventHubRepository(eventHubConnectionString, eventHubName, blobStorageConnectionString, blobContainerName);

        var result = eventHubRepository.SendDataAsync();

        Assert.True(result.Result);
    }

    [Fact]
    public void ShouldRecieveDataAsync()
    {
        string eventHubConnectionString = UserSecrets<EventHubRepositoryTest>.GetSecret("eventHubConnectionString");
        string eventHubName = "eventhubtest";

        string blobStorageConnectionString = UserSecrets<EventHubRepositoryTest>.GetSecret("blobStorageConnectionString");
        string blobContainerName = "eventhubtest";

        EventHubRepository eventHubRepository = new EventHubRepository(eventHubConnectionString, eventHubName, blobStorageConnectionString, blobContainerName);

        eventHubRepository.SendDataAsync();
        var result = eventHubRepository.RecieveDataAsync();

        Assert.True(result.Result);
    }
}