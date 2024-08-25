namespace Bon.SourceGeneration
{
    /// <summary>
    /// Represents a schema type with an annotation indicating whether it is nullable.
    /// </summary>
    internal readonly struct AnnotatedSchemaType
    {
        public SchemaType SchemaType { get; }
        public bool IsNullable { get; }

        public AnnotatedSchemaType(SchemaType schemaType, bool isNullable)
        {
            SchemaType = schemaType;
            IsNullable = isNullable;
        }

        public override string ToString()
        {
            return (IsNullable ? "Nullable" : "") + SchemaType;
        }
    }
}
