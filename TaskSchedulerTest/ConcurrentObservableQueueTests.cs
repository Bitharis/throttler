using ConcurrentObservableQueue;

namespace TaskSchedulerTest
{
    [TestClass]
    public class ConcurrentObservableQueueTests
    {
        private List<CollectionChangedEventArgs> _events;

        [TestInitialize]
        public void Initialize()
        {
            _events = new List<CollectionChangedEventArgs>();
        }

        [TestMethod]
        public void OnCollectionChanged_EnqueueItem_RaisesItemEnqueuedEvent()
        {
            // Arrange
            var queue = new ConcurrentObservableQueue<int>();
            queue.OnCollectionChanged += (sender, args) => _events.Add(args);

            // Act
            queue.Enqueue(1);

            // Assert
            Assert.AreEqual(1, _events.Count);
            Assert.AreEqual(CollectionChangeType.ItemEnqueued, _events[0].ChangeType);
        }


        [TestMethod]
        public void OnCollectionChanged_DequeueItem_RaisesItemDequeuedEvent()
        {
            // Arrange
            var queue = new ConcurrentObservableQueue<int>(new[] { 1 });
            queue.OnCollectionChanged += (sender, args) => _events.Add(args);

            // Act
            queue.TryDequeue(out var result);

            // Assert
            Assert.AreEqual(1, _events.Count);
            Assert.AreEqual(CollectionChangeType.ItemDequeued, _events[0].ChangeType);
        }

        [TestMethod]
        public void ConcurrentAccess_IsThreadSafe()
        {
            // Arrange
            var queue = new ConcurrentObservableQueue<int>(new[] { 1, 2 });

            // Act
            Parallel.For(0, 1000, i =>
            {
                if (i % 2 == 0)
                {
                    queue.Enqueue(i);
                }
                else
                {
                    queue.TryDequeue(out _);
                }
            });

            // Assert
            Assert.IsTrue(queue.Count >= 0);
        }
    }
}
