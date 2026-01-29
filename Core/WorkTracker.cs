namespace Core;

public class WorkTracker : IWorkTracker
{
    private readonly HashSet<string> _completed = new();
    private readonly HashSet<string> _failed = new();
    private readonly object _lock = new();

    public void RecordStarted(string workItemId)
    {
        // Optional: track in-progress items
    }

    public void RecordCompleted(string workItemId)
    {
        lock (_lock)
        {
            _completed.Add(workItemId);
        }
    }

    public void RecordFailed(string workItemId)
    {
        lock (_lock)
        {
            _failed.Add(workItemId);
        }
    }

    public int CompletedCount
    {
        get { lock (_lock) return _completed.Count; }
    }

    public int FailedCount
    {
        get { lock (_lock) return _failed.Count; }
    }

    public IReadOnlySet<string> CompletedItems
    {
        get { lock (_lock) return _completed.ToHashSet(); }
    }

    public IReadOnlySet<string> FailedItems
    {
        get { lock (_lock) return _failed.ToHashSet(); }
    }
}