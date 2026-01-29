namespace Workshop1;

using Core;
using System.Threading.Channels;

public class Workshop1Tests
{
[Fact]
public async Task ShouldProcessAllWorkItems()
{
    // Arrange
    var tracker = new WorkTracker();
    var workers = Enumerable.Range(1, 3)
        .Select(i => new ImageWorker($"Worker-{i}"))
        .ToList<IWorker>();
    
    var distributor = new NaiveWorkDistributor(workers, tracker);

    // Act
    var distributorTask = distributor.StartAsync(CancellationToken.None);

    var workItems = Enumerable.Range(1, 100)
        .Select(i => new ImageWorkItem(
            Id: $"IMG-{i:D3}",
            ImagePath: $"/images/photo-{i}.jpg",
            TargetSize: ImageSize.Thumbnail))
        .ToList();

    foreach (var item in workItems)
    {
        await distributor.EnqueueWorkAsync(item);
    }

    distributor.AllWorkAdded(); // Signal we're done adding all expected work

    await distributorTask; // Wait for all work to complete

    // Assert
    Assert.Equal(100, tracker.CompletedCount + tracker.FailedCount);
    Assert.Equal(0, tracker.FailedCount);
}
}
