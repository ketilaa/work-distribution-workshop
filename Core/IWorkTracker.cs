namespace Core;

public interface IWorkTracker
{
    void RecordStarted(string workItemId);
    void RecordCompleted(string workItemId);
    void RecordFailed(string workItemId);
    
    int CompletedCount { get; }
    int FailedCount { get; }
    IReadOnlySet<string> CompletedItems { get; }
    IReadOnlySet<string> FailedItems { get; }
}