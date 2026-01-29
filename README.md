# Distributed Workload Coordination Workshop Series

A hands-on workshop series exploring the challenges and patterns of distributed workload coordination, with a focus on understanding what happens behind the scenes when using public cloud services.

## Overview

When you use cloud services - uploading files, processing data, running serverless functions - multiple workers coordinate behind the scenes to handle your requests. This coordination looks simple from the outside, but involves solving complex problems around work distribution, failure handling, consistency, and scalability.

This workshop series takes you from the fundamentals to production-ready patterns, using C# and hands-on coding exercises to make these concepts concrete.

## What You'll Build

Throughout the workshops, you'll build increasingly sophisticated work distribution systems, starting with in-process coordination and progressing toward the patterns used by real cloud infrastructure:

- Work queues with retry logic
- Fair work distribution algorithms
- Leader election mechanisms
- Consensus-based coordination
- Production-ready resilience patterns

## Prerequisites

- **Language**: C# (intermediate level)
- **Concepts**: Familiarity with async/await, Tasks, and basic concurrency
- **Tools**: .NET 8.0 SDK or later
- **IDE**: Visual Studio, Visual Studio Code, or JetBrains Rider

## Workshop Structure

Each workshop builds on the previous one, introducing new coordination challenges and solutions:

### [Workshop 1: Introduction & Retry Logic](./Workshop1/README.md)
**Duration:** ~1.5 hours  
**Topics:**
- Why coordination is necessary at scale
- The problem of failed work items
- Implementing retry strategies (immediate, re-queue, exponential backoff)
- Dead letter queue pattern

**Real-world parallel:** AWS SQS, Azure Service Bus message processing

---

### Workshop 2: Work Distribution Algorithms
**Duration:** ~1.5 hours  
**Topics:**
- Fair work distribution strategies
- Round-robin vs work-stealing approaches
- Load-based distribution
- Handling backpressure when work arrives faster than processing capacity

**Real-world parallel:** Kubernetes pod scheduling, load balancer algorithms

---

### Workshop 3: Leader Election
**Duration:** ~1.5 hours  
**Topics:**
- Why you need a coordinator to coordinate
- Implementing basic leader election
- Handling leader failures and failover
- Split-brain scenarios and fencing

**Real-world parallel:** etcd, Consul, ZooKeeper cluster coordination

---

### Workshop 4: Multi-Pass Processing & Advanced Patterns
**Duration:** ~1.5 hours  
**Topics:**
- Consensus algorithms (Raft overview)
- Exactly-once vs at-least-once processing semantics
- Idempotency and state management
- Production trade-offs and CAP theorem implications

**Real-world parallel:** Distributed databases, container orchestration

## Getting Started

### Clone the Repository
```bash
git clone 
cd distributed-workload-coordination
```

### Verify Your Environment
```bash
dotnet --version  # Should be 8.0 or later
```

### Run Workshop 1
```bash
cd Workshop1
dotnet test
```

You should see output showing workers processing tasks, with some failures occurring.

## Repository Structure
```
distributed-workload-coordination/
├── README.md                          # This file
├── Core/
│   ├── IWorkItem.cs                   # Interface for work items
│   ├── IWorker.cs                     # Interface for workers
│   ├── IWorkDistributor.cs            # Interface for work distribution
│   ├── IWorkTracker.cs                # Interface for tracking work status
│   ├── IDeadLetterQueue.cs            # Interface for failed work (introduced later)
|   └── WorkTracker.cs                 # Work tracker implementation
├── Workshop1/
│   ├── README.md                      # Workshop 1 instructions
│   ├── ImageWorkItem.cs               # Concrete work item for image processing
│   ├── ImageWorker.cs                 # Worker that simulates image processing
│   ├── NaiveWorkDistributor.cs        # Basic distributor without retry logic
│   └── Workshop1Tests.cs              # Unit tests
├── Workshop2/
│   └── README.md                      # Coming soon
├── Workshop3/
│   └── README.md                      # Coming soon
└── Workshop4/
    └── README.md                      # Coming soon
```

## Key Concepts Covered

### Coordination Challenges
- **Race conditions**: When timing determines correctness
- **Failure detection**: Knowing when workers or work has failed
- **Consensus**: Getting multiple independent workers to agree on state
- **Fairness**: Distributing work evenly across workers
- **Consistency**: Ensuring all workers see the same view of work

### Cloud Service Patterns
- **Message queues**: Work distribution via queuing (SQS, Service Bus)
- **Dead letter queues**: Handling permanently failed items
- **Visibility timeouts**: Preventing duplicate processing
- **Auto-scaling**: Adding/removing workers dynamically
- **Health checks**: Detecting and replacing failed workers
- **Circuit breakers**: Preventing cascade failures

### Implementation Patterns
- **Work stealing**: Workers take work from others when idle
- **Exponential backoff**: Increasing delays between retries
- **Leader election**: One worker coordinates the others
- **Heartbeats**: Periodic signals that a worker is alive
- **Fencing tokens**: Preventing zombie workers from causing harm

## Learning Approach

Each workshop follows this structure:

1. **Context**: Why does this problem exist? Where do you see it in cloud services?
2. **Observe**: Run code that exhibits the problem
3. **Discuss**: What's happening? What could go wrong?
4. **Implement**: Build a solution together
5. **Test**: Verify the solution works
6. **Reflect**: What are the trade-offs? What's still imperfect?

The goal is not just to learn patterns, but to understand **why** these patterns exist and **when** to apply them.

## Real-World Relevance

Every concept in these workshops maps directly to production systems:

| Workshop Topic | AWS Services | Azure Services | Google Cloud |
|---------------|--------------|----------------|--------------|
| Work Queues | SQS, Kinesis | Service Bus, Event Hubs | Pub/Sub, Tasks |
| Worker Coordination | ECS, Lambda | Container Instances, Functions | Cloud Run, Functions |
| Leader Election | ECS Service Discovery | Service Fabric | GKE Leader Election |
| Distributed State | DynamoDB, RDS | Cosmos DB, SQL Database | Firestore, Spanner |
| Auto-scaling | Auto Scaling Groups | VM Scale Sets | Managed Instance Groups |

## Philosophy

These workshops embrace **learning through building**. Rather than just explaining how distributed systems work, we:

- Start with simple, broken implementations
- Observe failures firsthand
- Incrementally fix problems together
- Discuss trade-offs at each step

The code is intentionally kept simple (in-process, simulated failures) so you can focus on coordination logic rather than infrastructure complexity.

## Target Audience

These workshops are designed for:

- **Backend developers** wanting to understand how cloud services work internally
- **DevOps engineers** who deploy and operate distributed systems
- **Solution architects** designing scalable cloud architectures
- **Anyone curious** about what makes distributed systems hard

You don't need prior distributed systems experience - just curiosity and willingness to think through problems.

## Contributing

Found an issue or have a suggestion? Contributions are welcome!

- Report bugs or unclear instructions via Issues
- Suggest improvements via Pull Requests
- Share your workshop experience in Discussions

## Additional Resources

### Books
- "Designing Data-Intensive Applications" by Martin Kleppmann
- "Database Internals" by Alex Petrov
- "Understanding Distributed Systems" by Roberto Vitillo

### Online Resources
- [AWS Architecture Blog](https://aws.amazon.com/blogs/architecture/)
- [Azure Architecture Center](https://learn.microsoft.com/en-us/azure/architecture/)
- [Jepsen: Distributed Systems Safety Research](https://jepsen.io/)
- [The Paper Trail: Distributed Systems Blog](https://www.the-paper-trail.org/)

### Academic Papers
- "The Raft Consensus Algorithm" (Ongaro & Ousterhout)
- "Paxos Made Simple" (Leslie Lamport)
- "Time, Clocks, and the Ordering of Events in a Distributed System" (Lamport)

## License

This workshop series is licensed under [CC0 1.0 Universal (CC0 1.0) Public Domain Dedication](https://creativecommons.org/publicdomain/zero/1.0/).

You can copy, modify, distribute and perform the work, even for commercial purposes, all without asking permission. See the [LICENSE](./LICENSE) file for details.

---

**Ready to start?** Head to [Workshop 1](./Workshop1/README.md) to begin your journey into distributed workload coordination!

**Questions or feedback?** Open an issue or start a discussion - we'd love to hear from you.