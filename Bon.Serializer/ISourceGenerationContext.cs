namespace Bon.Serializer;

public interface ISourceGenerationContext
{
    //2at
    void LoadSchemas(Action<Type, Schema> onSchemaLoaded);

    //2at
    void Run(BonFacade bonFacade);
}
