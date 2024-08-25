namespace Bon.Serializer;

public sealed class DeserializationFailedException(string? message) : Exception(message) { }
