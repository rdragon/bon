﻿namespace Bon.Serializer.Serialization;

/// <param name="Write">A delegate of the form <c>Action&lt;BinaryWriter, T&gt;</c>.</param>
/// <param name="UsesCustomSchemas">
/// Whether the writer might use custom schemas.
/// This value controls whether the header of the final binary message includes a block ID.
/// </param>
/// <param name="SimpleWriterType">
/// If different from <see cref="SimpleWriterType.None"/> then <see cref="SimpleWriterStore"/> instead of <see cref="WriterStore"/> can
/// be used to write the message.
/// </param>
internal readonly record struct Writer(Delegate Write, bool UsesCustomSchemas, SimpleWriterType SimpleWriterType = SimpleWriterType.None)
{
    public Writer<T> Convert<T>() => new((Action<BinaryWriter, T>)Write, UsesCustomSchemas, SimpleWriterType);
}

/// <param name="Write">Writes a value of type <typeparamref name="T"/> to the stream.</param>
/// <param name="UsesCustomSchemas">
/// Whether the writer might use custom schemas.
/// This value controls whether the header of the final binary message includes a block ID.
/// </param>
/// <param name="SimpleWriterType">
/// If different from <see cref="SimpleWriterType.None"/> then <see cref="SimpleWriterStore"/> instead of <see cref="WriterStore"/> can
/// be used to write the message.
/// </param>
internal readonly record struct Writer<T>(Action<BinaryWriter, T> Write, bool UsesCustomSchemas, SimpleWriterType SimpleWriterType);
