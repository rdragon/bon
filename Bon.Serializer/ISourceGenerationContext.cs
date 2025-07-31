namespace Bon.Serializer;

public interface ISourceGenerationContext
{
    /// <summary>
    /// Generates schemas and calls the <paramref name="onSchemaLoaded"/> for every record, union and enum.
    /// For records and enums the callback is called twice, once for the nullable version and once for the non-nullable version.
    /// For non-nullable classes the type is wrapped in a <see cref="NotNull{T}"/> to distinguish it from the nullable version.
    /// </summary>
    void LoadSchemas(Action<Type, Schema> onSchemaLoaded);

    /// <summary>
    /// Fills the stores by calling the methods of the <paramref name="bonFacade"/>.
    /// </summary>
    void Run(BonFacade bonFacade);
}
