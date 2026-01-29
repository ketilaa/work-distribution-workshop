# Workshop 1: Introduction to Distributed Workload Coordination

## Overview

Welcome to the first workshop in our series on distributed workload coordination! In this workshop, we'll explore the fundamental challenges that arise when multiple workers need to coordinate to process a shared queue of work - the same problems that cloud providers solve millions of times per second behind the scenes.

## What You'll Learn

- Why coordination is necessary at scale
- How cloud services distribute work across multiple workers
- The problem of failed work items and why retries matter
- Implementing basic retry logic with different strategies

## The Problem Domain

When you upload 100 photos to a cloud service, resize a batch of videos, or trigger a large data processing job, the cloud provider doesn't use a single machine to process your request. Instead:

1. Your work is broken into **individual work items** (e.g., one photo to resize)
2. These items are placed in a **work queue**
3. Multiple **worker instances** pull items from the queue and process them in parallel
4. The system coordinates to ensure work is distributed fairly and nothing is lost

This pattern appears everywhere in cloud infrastructure:

- **AWS Lambda / Azure Functions**: Distributing function invocations across compute instances
- **Amazon SQS / Azure Service Bus**: Multiple consumers processing messages from a queue
- **Kubernetes**: Scheduling pods across worker nodes
- **Auto-scaling groups**: Distributing incoming HTTP requests across instances
- **Data processing pipelines**: Parallel processing of large datasets

## Why This Is Hard

What seems simple - "just have multiple workers pull from a queue" - becomes complex when you consider:

1. **Failures are inevitable**: Workers crash, network connections drop, operations timeout
2. **No central coordinator**: Workers operate independently and don't know what others are doing
3. **Race conditions**: Multiple workers might try to grab the same work simultaneously
4. **Partial failures**: What happens when a worker starts processing but fails midway?
5. **Lost work**: If a worker fails while processing, how do you ensure that work isn't lost?

## Today's Challenge: Handling Failed Work

In this workshop, we'll focus on one specific problem: **what happens when work items fail?**

Consider our image processing scenario:
- You upload 100 images
- Worker-3 picks up image #47
- Halfway through resizing, the worker encounters corrupted image data and fails
- What happens to image #47?

Without proper retry logic, that work is simply **lost**. The user's photo never gets processed, and they might not even know it failed.

## The Code Structure

We've provided starter code with these components:

### Interfaces

**`IWorkItem`** - Represents a unit of work to be processed
```csharp
public interface IWorkItem
{
    string Id { get; }
    string Description { get; }
}
```

**`IWorker`** - Processes work items
```csharp
public interface IWorker
{
    string WorkerId { get; }
    Task ProcessAsync(IWorkItem workItem, CancellationToken cancellationToken);
}
```

**`IWorkDistributor`** - Distributes work to workers
```csharp
public interface IWorkDistributor
{
    Task StartAsync(CancellationToken cancellationToken);
    Task EnqueueWorkAsync(IWorkItem workItem);
    void CompleteAdding();
}
```

**`IWorkTracker`** - Tracks work completion and failures
```csharp
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
```

### Implementations

**`ImageWorker`** - Simulates image processing with:
- Random processing time (100-500ms) to simulate real work
- 10% random failure rate to simulate real-world failures (network issues, corrupted data, etc.)

**`NaiveWorkDistributor`** - A basic implementation that:
- Creates a Channel for the work queue
- Starts multiple workers that pull from the queue
- Currently has **no retry logic** - failed work is lost!

**`WorkTracker`** - Tracks which items completed successfully vs failed

## Workshop Activities

### Part 1: Observe the Problem (10 minutes)

1. Run the provided unit test multiple times:
```bash
   dotnet test
```

2. Watch the console output. You'll see:
   - Workers starting and completing work
   - Occasional failures: `Error in Worker-X: [Worker-X] Failed to process IMG-XXX`
   - The test sometimes passes, sometimes fails

3. **Discussion Questions:**
   - What happens to work items that fail?
   - How would a user know their photo failed to process?
   - Is "90% success rate" acceptable for a cloud service?
   - What would happen in production with thousands of work items?

### Part 2: Implement Retry Logic (30 minutes)

We'll implement retry logic together, exploring three approaches:

#### Strategy 1: Immediate Retry (Simple but Blocking)
The worker retries failed items immediately, up to a maximum number of attempts.

**Pros:**
- Simple to implement
- Failed work gets retried quickly

**Cons:**
- Worker is blocked during retries (can't process other work)
- No backoff - might overwhelm a struggling service

#### Strategy 2: Re-queue Failed Items (Better Throughput)
Failed items are placed back in the work queue to be retried later.

**Pros:**
- Workers don't block - they can process other work
- Failed items get another chance

**Cons:**
- Failed items compete with new work
- No delay - might retry immediately and fail again
- Need to track retry count to avoid infinite loops

#### Strategy 3: Exponential Backoff (Production-Ready)
Failed items are retried with increasing delays between attempts.

**Pros:**
- Gives transient failures time to resolve (network blip, temporary overload)
- Prevents retry storms
- Matches what production systems do

**Cons:**
- More complex to implement
- Requires separate retry handling mechanism

### Part 3: Testing Your Solution (10 minutes)

Modify the unit test to verify your retry logic:
```csharp
[Fact]
public async Task ShouldRetryAndCompleteAllWork()
{
    // Arrange
    var tracker = new WorkTracker();
    var workers = Enumerable.Range(1, 3)
        .Select(i => new ImageWorker($"Worker-{i}"))
        .ToList<IWorker>();
    
    var distributor = new ImprovedWorkDistributor(workers, tracker, maxRetries: 3);

    // Act
    var distributorTask = distributor.StartAsync(CancellationToken.None);

    for (int i = 1; i <= 20; i++)
    {
        await distributor.EnqueueWorkAsync(new ImageWorkItem(
            Id: $"IMG-{i:D3}",
            ImagePath: $"/images/photo-{i}.jpg",
            TargetSize: ImageSize.Thumbnail));
    }

    distributor.CompleteAdding();
    await distributorTask;

    // Assert
    // With retries, we should eventually succeed on most/all items
    Assert.True(tracker.CompletedCount >= 18); // Allow for some permanent failures
    Assert.True(tracker.FailedCount <= 2);
}
```

## Real-World Parallels

The patterns we're exploring map directly to cloud services:

| Our Code | AWS | Azure | Pattern |
|----------|-----|-------|---------|
| Work Queue | SQS | Service Bus Queue | Message Queue |
| Worker | Lambda/EC2 | Function/VM | Compute Instance |
| Retry Count | Message Receive Count | Delivery Count | Retry Tracking |
| Max Retries | maxReceiveCount | Max Delivery Count | Retry Limit |
| Failed Items Tracker | Dead Letter Queue | Dead Letter Queue | DLQ Pattern |
| Exponential Backoff | Visibility Timeout | Lock Duration | Backoff Strategy |

## Key Takeaways

1. **Failures are normal** - Design for them, don't just hope they won't happen
2. **Retries are essential** - Most transient failures resolve on retry
3. **Backoff prevents storms** - Immediate retries can make problems worse
4. **Limits prevent infinite loops** - Always have a maximum retry count
5. **Observability matters** - You need to know when and why work fails

## What's Next?

In Workshop 2, we'll tackle:
- **Work distribution fairness** - Ensuring workers share load evenly
- **Different distribution strategies** - Round-robin, work-stealing, load-based
- **Backpressure handling** - What happens when work arrives faster than you can process it?

## Discussion Questions

Before we wrap up, let's discuss:

1. What's the right number of retries? Does it depend on the type of failure?
2. Should all failures be retried, or only certain types?
3. How would you handle "poison messages" - items that will always fail?
4. What information should you log when work fails?
5. How would you alert operators that many items are failing?

## Resources

- [AWS SQS Dead Letter Queues](https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-dead-letter-queues.html)
- [Azure Service Bus Message Sessions](https://learn.microsoft.com/en-us/azure/service-bus-messaging/message-sessions)
- [Exponential Backoff and Jitter](https://aws.amazon.com/blogs/architecture/exponential-backoff-and-jitter/)
- [Patterns for Resilient Architecture](https://learn.microsoft.com/en-us/azure/architecture/patterns/retry)

---

**Time allocation:**
- Introduction and context: 15 minutes
- Part 1 (Observe): 15 minutes
- Part 2 (Implement): 45 minutes
- Part 3 (Test): 20 minutes
- Discussion and wrap-up: 15 minutes

**Total: ~2 hours**