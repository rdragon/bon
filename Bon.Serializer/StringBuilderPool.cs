namespace Bon.Serializer;

internal class StringBuilderPool
{
    private const int MaxStringBuilderCapacity = 512;
    private const int PoolSize = 8;

    private readonly StringBuilder?[] _pool = new StringBuilder?[PoolSize];
    private int _index;
    private SpinLock _lock = new();

    public StringBuilder Rent()
    {
        // This method is similar to the method System.Buffers.ConfigurableArrayPool<T>.Bucket.Rent.

        StringBuilder? stringBuilder = null;
        var lockTaken = false;

        try
        {
            _lock.Enter(ref lockTaken);

            if (_index < PoolSize)
            {
                stringBuilder = _pool[_index];
                _pool[_index++] = null;
            }
        }
        finally
        {
            if (lockTaken)
            {
                _lock.Exit();
            }
        }

        return stringBuilder ?? new StringBuilder();
    }

    public void Return(StringBuilder stringBuilder)
    {
        if (stringBuilder.Capacity > MaxStringBuilderCapacity)
        {
            return;
        }

        stringBuilder.Clear();
        var lockTaken = false;

        try
        {
            _lock.Enter(ref lockTaken);

            if (_index != 0)
            {
                _pool[--_index] = stringBuilder;
            }
        }
        finally
        {
            if (lockTaken)
            {
                _lock.Exit();
            }
        }
    }

    public static StringBuilderPool Shared { get; } = new();
}
