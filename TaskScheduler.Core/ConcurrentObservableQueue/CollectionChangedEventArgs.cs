namespace ConcurrentObservableQueue
{
    public class CollectionChangedEventArgs : EventArgs
    {
        public CollectionChangedEventArgs(CollectionChangeType changeType)
        {
            this.ChangeType = changeType;
        }

        public CollectionChangeType ChangeType { get; private set; }
    }
}
