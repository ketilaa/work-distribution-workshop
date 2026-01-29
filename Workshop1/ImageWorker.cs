namespace Workshop1;

using Core;

public class ImageWorker : IWorker
{
    private readonly Random _random = new();
    
    public string WorkerId { get; }

    public ImageWorker(string workerId)
    {
        WorkerId = workerId;
    }

    public async Task ProcessAsync(IWorkItem workItem, CancellationToken cancellationToken)
    {
        if (workItem is not ImageWorkItem imageWork)
            throw new ArgumentException("Expected ImageWorkItem");

        Console.WriteLine($"[{WorkerId}] Starting: {imageWork.Description}");
        
        // Simulate processing time (100-500ms)
        await Task.Delay(_random.Next(100, 500), cancellationToken);
        
        // Simulate occasional failures (10% chance)
        if (_random.Next(100) < 10)
        {
            throw new InvalidOperationException($"[{WorkerId}] Failed to process {imageWork.Id}");
        }
        
        Console.WriteLine($"[{WorkerId}] Completed: {imageWork.Description}");
    }
}