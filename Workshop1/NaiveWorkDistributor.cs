using Core;
using System.Threading.Channels;

public class NaiveWorkDistributor : IWorkDistributor
{
    private readonly Channel<IWorkItem> _workChannel = Channel.CreateUnbounded<IWorkItem>();
    private readonly IReadOnlyList<IWorker> _workers;
    private readonly IWorkTracker? _workTracker;

    public NaiveWorkDistributor(
        IEnumerable<IWorker> workers,
        IWorkTracker? workTracker = null)
    {
        _workers = workers.ToList();
        _workChannel = Channel.CreateUnbounded<IWorkItem>();
        _workTracker = workTracker;
    }

    public void AllWorkAdded()
    {
        _workChannel.Writer.Complete();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var workerTasks = _workers.Select(worker => 
            RunWorkerAsync(worker, cancellationToken)).ToList();

        await Task.WhenAll(workerTasks);
    }

    private async Task RunWorkerAsync(IWorker worker, CancellationToken cancellationToken)
    {
        await foreach (var workItem in _workChannel.Reader.ReadAllAsync(cancellationToken))
        {
            _workTracker?.RecordStarted(workItem.Id);
            
            try
            {
                await worker.ProcessAsync(workItem, cancellationToken);
                _workTracker?.RecordCompleted(workItem.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {worker.WorkerId}: {ex.Message}");
                _workTracker?.RecordFailed(workItem.Id);
            }
        }
    }

    public async Task EnqueueWorkAsync(IWorkItem workItem)
    {
        await _workChannel.Writer.WriteAsync(workItem);
    }
}