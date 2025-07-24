using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    /// <summary>
    /// Represents a type that can be serialized.
    /// </summary>
    internal interface IDefinition : IRecursiveEquatable
    {
        /// <summary>
        /// The type that this definition represents.
        /// A value like "ExampleNamespace.ExampleClass?" or "int" or "System.IEnumerable&lt;string?&gt;".
        /// Two definitions with the same <see cref="Type"/> are considered equal (assuming they were created from the same version
        /// of the code).
        /// Reference types always end with a question mark.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Equal to <see cref="Type"/> without the question mark at the end.
        /// </summary>
        string TypeNonNullable { get; }

        /// <summary>
        /// The schema type that will be used when serializing this type.
        /// This property is only used when obtaining the schema corresponding to this definition, and to determine nullability.
        /// (And it is used when comparing two definitions.)
        /// Therefore, if you have a custom way to obtain the schema, you can set this property to anything you like.
        /// </summary>
        SchemaType SchemaType { get; }

        /// <summary>
        /// Returns whether this is a nullable value type or (any) reference type.
        /// </summary>
        bool IsNullable { get; }

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

        string ToPrettyString(bool allowRecursion = true);

        string TypeForWriter { get; }
    }

    /// <summary>
    /// A <see cref="RecordDefinition"/>, <see cref="UnionDefinition"/>, or <see cref="EnumDefinition"/>.
    /// For these types the source generation is critical.
    /// Other types can also be created using reflection.
    /// </summary>
    internal interface ICriticalDefinition : IDefinition
    {
        ICriticalDefinition SwapNullability();
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
