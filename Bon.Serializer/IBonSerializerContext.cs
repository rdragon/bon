namespace Bon.Serializer;

public interface IBonSerializerContext
{
    ISourceGenerationContext SourceGenerationContext =>
        throw new InvalidOperationException(
            $"The source generator did not find the class {GetType()}. " +
            $"Perhaps this class is missing the BonSerializerContext attribute?");
}
