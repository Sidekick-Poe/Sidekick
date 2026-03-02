using Sidekick.Modules.Logs;
using Xunit;

namespace Sidekick.Common.Tests.Logging;

public class CappedQueueTests
{
    [Fact]
    public void Enqueue_AddsItem()
    {
        var queue = new CappedQueue<string>(3);
        queue.Enqueue("A");
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void Enqueue_CapsAtCapacity()
    {
        var queue = new CappedQueue<string>(3);
        queue.Enqueue("A");
        queue.Enqueue("B");
        queue.Enqueue("C");
        queue.Enqueue("D");

        Assert.Equal(3, queue.Count);
        var items = queue.ToList();
        Assert.Equal(new[] { "B", "C", "D" }, items);
    }

    [Fact]
    public void Dequeue_RemovesItem()
    {
        var queue = new CappedQueue<string>(3);
        queue.Enqueue("A");
        var item = queue.Dequeue();
        Assert.Equal("A", item);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Dequeue_EmptyQueue_ThrowsException()
    {
        var queue = new CappedQueue<string>(3);
        Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
    }
}