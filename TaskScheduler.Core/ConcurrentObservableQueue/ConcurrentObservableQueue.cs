using System.Collections.Concurrent;

namespace ConcurrentObservableQueue
{
    public sealed class ConcurrentObservableQueue<T> : IConcurrentObservableQueue<T>
    {
        private ConcurrentQueue<T> queue;

        /// <summary>
        /// Event raised when the contents of the ConcurrentObservableQueue have changed.
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs>? OnCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the ConcurrentObservableQueue<T> class.
        /// </summary>
        public ConcurrentObservableQueue()
        {
            queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrentObservableQueue<T> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection"></param>
        public ConcurrentObservableQueue(IEnumerable<T> collection)
        {
            queue = new ConcurrentQueue<T>(collection);
        }

        /// <summary>
        /// Gets the number of elements contained in the ConcurrentObservableQueue<T>.
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// Gets a value that indicates whether the ConcurrentObservableQueue<T> is empty.
        /// </summary>
        public bool IsEmpty => queue.IsEmpty;

        /// <summary>
        /// Removes all objects from the ConcurrentObservableQueue<T>.
        /// </summary>
        public void Clear()
        {
            queue.Clear();
        }

        /// <summary>
        /// Adds an object to the end of the ConcurrentObservableQueue.
        /// </summary>
        public void Enqueue(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
                if (queue.Count == 1)
                {
                    OnCollectionChanged?.Invoke(this, new CollectionChangedEventArgs(CollectionChangeType.FirstItemEnqueued));
                }
                else
                {
                    OnCollectionChanged?.Invoke(this, new CollectionChangedEventArgs(CollectionChangeType.ItemEnqueued));
                }
            }            
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the ConcurrentObservableQueue.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if an element was removed and returned from the beginning of the ConcurrentObservableQueue<T> successfully; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool TryDequeue(out T result)
        {
            bool isDequeueSuccessful = false;
            isDequeueSuccessful = queue.TryDequeue(out result);
            OnCollectionChanged?.Invoke(this, new CollectionChangedEventArgs(CollectionChangeType.ItemDequeued));
            return isDequeueSuccessful;
        }

        /// <summary>
        /// Tries to return an object from the beginning of the ConcurrentObservableQueue<T> without removing it.
        /// </summary>
        /// <param name="result">The dequeued item.</param>
        /// <returns>
        /// <see langword="true" /> f an object was returned successfully;  
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool TryPeek(out T result)
        {
            return queue.TryPeek(out result);
        }

        //
        // Summary:
        //     Determines whether a sequence contains a specified element by using the default
        //     equality comparer.
        //
        // Parameters:
        //   source:
        //     A sequence in which to locate a value.
        //
        //   value:
        //     The value to locate in the sequence.
        //
        // Type parameters:
        //   TSource:
        //     The type of the elements of source.
        //
        // Returns:
        //     true if the source sequence contains an element that has the specified value;
        //     otherwise, false.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     source is null.
        public bool Contains(T item) => queue.Contains(item);
    }
}
