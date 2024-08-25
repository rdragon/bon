namespace Bon.Serializer.Deserialization;

public delegate T Read<out T>(BonInput input);
