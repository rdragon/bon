namespace Bon.Serializer;

public interface ISourceGenerationContext
{
    void UpdateSchemaStore(BonFacade bonFacade);

    void Run(BonFacade bonFacade);
}
