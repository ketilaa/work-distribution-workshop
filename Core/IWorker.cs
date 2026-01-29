namespace Core;

public interface IWorker
{
    string WorkerId { get; }
    Task ProcessAsync(IWorkItem workItem, CancellationToken cancellationToken);
}
