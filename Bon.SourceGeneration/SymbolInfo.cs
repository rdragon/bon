using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Bon.SourceGeneration
{
    internal sealed class SymbolInfo
    {
        /// <summary>
        /// For value types the non-nullable version of the type.
        /// </summary>
        public ITypeSymbol Symbol { get; set; }

        public string Type { get; set; }

        public bool IsNullable { get; set; }

        public string FullName { get; set; }

        public ImmutableArray<ITypeSymbol> TypeArguments { get; set; }

        public ArrayInfo TryGetArrayInfo()
        {
            switch (FullName)
            {
                case "System.Array": return new ArrayInfo(ReadCollectionType.Array, CollectionType.Array);
                case "System.Collections.Generic.List`1": return new ArrayInfo(ReadCollectionType.List, CollectionType.List);
                case "System.Collections.Generic.IList`1": return new ArrayInfo(ReadCollectionType.List, CollectionType.List);
                case "System.Collections.Generic.IReadOnlyList`1": return new ArrayInfo(ReadCollectionType.Array, CollectionType.List);
                case "System.Collections.Generic.ICollection`1": return new ArrayInfo(ReadCollectionType.List, CollectionType.IEnumerable);
                case "System.Collections.Generic.IReadOnlyCollection`1": return new ArrayInfo(ReadCollectionType.Array, CollectionType.IEnumerable);
                case "System.Collections.Generic.IEnumerable`1": return new ArrayInfo(ReadCollectionType.Array, CollectionType.IEnumerable);
                default: return null;
            }
        }

        public DictionaryType? TryGetDictionaryType()
        {
            switch (FullName)
            {
                case "System.Collections.Generic.Dictionary`2": return DictionaryType.Dictionary;
                case "System.Collections.Generic.IDictionary`2": return DictionaryType.IDictionary;
                case "System.Collections.Generic.IReadOnlyDictionary`2": return DictionaryType.IReadOnlyDictionary;
                default: return null;
            }
        }

        public ITypeSymbol TryGetEnumUnderlyingSymbol()
        {
            if (Symbol.TypeKind != TypeKind.Enum)
            {
                return null;
            }

            return (Symbol as INamedTypeSymbol)?.EnumUnderlyingType;
        }

        public INamedTypeSymbol GetNamedTypeSymbol()
        {
            return Symbol as INamedTypeSymbol ?? throw new SourceGenerationException($"Cannot handle '{Symbol}'.", 8468, Symbol);
        }
    }

    internal sealed class ArrayInfo
    {
        public ReadCollectionType ReadCollectionType { get; set; }
        public CollectionType CollectionType { get; set; }

        public ArrayInfo(ReadCollectionType readCollectionType, CollectionType collectionType)
        {
            ReadCollectionType = readCollectionType;
            CollectionType = collectionType;
        }
    }
}
