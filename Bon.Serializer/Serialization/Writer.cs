namespace Bon.Serializer.Serialization;

/// <param name="Write">A delegate of the form <c>Action&lt;BinaryWriter, T&gt;</c>.</param>
/// <param name="SimpleWriterType">
/// If different from <see cref="SimpleWriterType.None"/> then <see cref="SimpleWriter"/> instead of <see cref="WriterStore"/> can
/// be used to write the message.
/// </param>
internal readonly record struct Writer(Delegate Write, SimpleWriterType SimpleWriterType = SimpleWriterType.None)
{
    public Writer<T> Convert<T>() => new((Action<BinaryWriter, T>)Write, SimpleWriterType);
}

/// <param name="Write">Writes a value of type <typeparamref name="T"/> to the stream.</param>
/// <param name="SimpleWriterType">
/// If different from <see cref="SimpleWriterType.None"/> then <see cref="SimpleWriter"/> instead of <see cref="WriterStore"/> can
/// be used to write the message.
/// </param>
internal readonly record struct Writer<T>(Action<BinaryWriter, T> Write, SimpleWriterType SimpleWriterType);
