namespace Core;

public interface IWorkDistributor
{
    Task StartAsync(CancellationToken cancellationToken);
    Task EnqueueWorkAsync(IWorkItem workItem);
    void AllWorkAdded();
}
