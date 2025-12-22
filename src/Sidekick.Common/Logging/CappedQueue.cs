namespace Sidekick.Common.Logging;

public class CappedQueue<T>
{
    private readonly Queue<T> queue;
    private readonly int capacity;
    private readonly object lockObject = new();

    public CappedQueue(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }

        this.capacity = capacity;
        this.queue = new Queue<T>(capacity);
    }

    public void Enqueue(T item)
    {
        lock (lockObject)
        {
            if (queue.Count == capacity)
            {
                queue.Dequeue();
            }
            queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        lock (lockObject)
        {
            if (queue.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }
            return queue.Dequeue();
        }
    }

    public IEnumerable<T> ToList()
    {
        lock (lockObject)
        {
            return new List<T>(queue);
        }
    }

    public int Count
    {
        get
        {
            lock (lockObject)
            {
                return queue.Count;
            }
        }
    }
}
