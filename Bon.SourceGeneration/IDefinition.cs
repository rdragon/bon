using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Represents a type that can be serialized.
    /// </summary>
    internal interface IDefinition : IRecursiveEquatable
    {
        /// <summary>
        /// A value like "ExampleNamespace.ExampleClass?" or "int" or "System.IEnumerable&lt;string?&gt;".
        /// Two definitions with the same <see cref="Type"/> are considered equal (assuming they were created from the same version
        /// of the code).
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Equal to <see cref="Type"/> without the question mark at the end.
        /// </summary>
        string TypeNonNullable { get; }

        SchemaType SchemaType { get; }

        bool IsNullable { get; }

        AnnotatedSchemaType AnnotatedSchemaType { get; }

        /// <summary>
        /// Returns e.g. "typeof(ExampleNamespace.ExampleClass)".
        /// </summary>
        string TypeOf { get; }

        IDefinition ToNullable();

        bool IsReferenceType { get; }

        bool IsValueType { get; }

        /// <summary>
        /// Returns the definitions of the type parameters for generic types.
        /// Returns the definitions of the members for record types and union types.
        /// Returns an empty array for the other types.
        /// </summary>
        IEnumerable<IDefinition> GetInnerDefinitions();

        /// <summary>
        /// The class that contains the create method that creates the schema for this definition.
        /// </summary>
        string SchemaBaseClass { get; }
    }

    /// <summary>
    /// A <see cref="RecordDefinition"/>, <see cref="UnionDefinition"/>, or <see cref="EnumDefinition"/>.
    /// These are all the types for which the serialization and deserialization is dependent on the source generated code.
    /// For other types, like arrays and tuples, reflection is used for (at least part of) the serialization and deserialization.
    /// </summary>
    internal interface IMajorDefinition : IDefinition
    {
        IMajorDefinition ToNonNullable();
    }

    /// <summary>
    /// A <see cref="RecordDefinition"/> or <see cref="UnionDefinition"/>.
    /// </summary>
    internal interface ICustomDefinition : IDefinition
    {
        /// <summary>
        /// The members of the definition, ordered by ID.
        /// </summary>
        IReadOnlyList<IMember> Members { get; }
    }

    /// <summary>
    /// A <see cref="Member"/> or <see cref="UnionMember"/>.
    /// </summary>
    internal interface IMember : IRecursiveEquatable
    {
        int Id { get; }

        IDefinition Definition { get; }
    }

    internal interface ITupleDefinition : IDefinition { };

    /// <summary>
    /// Defines an equality and hash code method that keep track of ancestors to prevent infinite recursion.
    /// </summary>
    internal interface IRecursiveEquatable
    {
        bool Equals(object obj, AncestorCollection ancestors);

        void AppendHashCode(AncestorCollection ancestors, ref int hashCode);
    }
}
