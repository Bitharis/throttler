
namespace ConcurrentObservableQueue
{
    public interface IConcurrentObservableQueue<T>
    {
        int Count { get; }
        bool IsEmpty { get; }

        event EventHandler<CollectionChangedEventArgs>? OnCollectionChanged;

        void Clear();
        bool Contains(T item);
        void Enqueue(T item);
        bool TryDequeue(out T result);
        bool TryPeek(out T result);
    }
}