using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    /// <summary>
    /// Represents any collection type, e.g. <see cref="List{T}"/> or <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal sealed class ArrayDefinition : Definition
    {
        public IDefinition ElementDefinition { get; }

        /// <summary>
        /// The collection type that is used during deserialization.
        /// When this definition is deserialized its actual type will be the read collection type.
        /// The read collection type should be implicitly convertible to the collection type of this definition.
        /// </summary>
        public ReadCollectionType ReadCollectionType { get; }

        /// <summary>
        /// The simplified collection type of this definition.
        /// If this definition does not support an indexer then the collection type is <see cref="CollectionType.IEnumerable"/>.
        /// Otherwise the collection type depends on whether a Count or Length property is available.
        /// </summary>
        public CollectionType CollectionType { get; }

        public ArrayDefinition(
            string type,
            SchemaType schemaType,
            IDefinition elementDefinition,
            ReadCollectionType readCollectionType,
            CollectionType collectionType) : base(type, schemaType)
        {
            ElementDefinition = elementDefinition;
            ReadCollectionType = readCollectionType;
            CollectionType = collectionType;
        }

        // No need to override Equals and GetHashCode because the base implementation is sufficient.
        // ReadCollectionType and CollectionType are determined by Type.

        public string GetConstructor(string count)
        {
            if (ReadCollectionType == ReadCollectionType.Array)
            {
                return $"new {ElementDefinition.Type}[{count}]";
            }

            if (ReadCollectionType == ReadCollectionType.List)
            {
                return $"new List<{ElementDefinition.Type}>({count})";
            }

            throw new InvalidOperationException();
        }

        public override IEnumerable<IDefinition> GetInnerDefinitions()
        {
            yield return ElementDefinition;
        }

        public override string SchemaBaseClass => "GenericSchema1";
    }

    /// <summary>
    /// Array, List or IEnumerable.
    /// The simplified collection type of an <see cref="ArrayDefinition"/>.
    /// </summary>
    public enum CollectionType
    {
        Array,
        List,
        IEnumerable,
    }

    /// <summary>
    /// Array or List.
    /// The collection type that is used during deserialization.
    /// When an <see cref="ArrayDefinition"/> is deserialized its actual type will be the read collection type.
    /// </summary>
    public enum ReadCollectionType
    {
        Array,
        List,
    }
}
