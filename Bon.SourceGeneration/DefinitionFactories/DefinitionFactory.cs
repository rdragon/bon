using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration.DefinitionFactories
{
    internal sealed class DefinitionFactory
    {
        /// <summary>
        /// Keeps track of all the definitions that are currently being built.
        /// </summary>
        private readonly Dictionary<string, IDefinition> _ancestors = new Dictionary<string, IDefinition>();

        private readonly RecordDefinitionFactory _recordDefinitionFactory;
        private readonly UnionDefinitionFactory _unionDefinitionFactory;

        public DefinitionFactory()
        {
            _recordDefinitionFactory = new RecordDefinitionFactory(this);
            _unionDefinitionFactory = new UnionDefinitionFactory(this);
        }

        public void Clear() => _ancestors.Clear();

        public IDefinition GetDefinition(ITypeSymbol symbol)
        {
            symbol = symbol.UpdateNullability();

            return
                TryGetArrayDefinition(symbol) ??
                GetDefinition(symbol.ToNamedTypeSymbol());
        }

        private IDefinition GetDefinition(INamedTypeSymbol symbol)
        {
            var type = symbol.GetTypeName();

            if (_ancestors.TryGetValue(type, out var ancestor))
            {
                return ancestor;
            }

            return
                NativeDefinition.TryGetNativeDefinition(type) ??
                TryGetEnumDefinition(symbol) ??
                TryGetDictionaryDefinition(symbol) ??
                TryGetTuple2Definition(symbol) ??
                TryGetTuple3Definition(symbol) ??
                _unionDefinitionFactory.TryGetUnionDefinition(symbol) ??
                GetRecordDefinition(symbol);
        }

        private RecordDefinition GetRecordDefinition(INamedTypeSymbol symbol)
        {
            var type = symbol.GetTypeName();
            var recordDefinition = _recordDefinitionFactory.GetRecordDefinitionWithoutMembers(symbol);

            _ancestors.Add(type, recordDefinition);
            _recordDefinitionFactory.AddMembers(recordDefinition, symbol);
            _ancestors.Remove(type);

            return recordDefinition;
        }

        private ArrayDefinition TryGetArrayDefinition(ITypeSymbol symbol)
        {
            var type = symbol.GetTypeName();

            if (type == "System.Guid" || type == "System.Guid?")
            {
                return new ArrayDefinition(type,
                    SchemaType.Array,
                    NativeDefinition.GetNativeDefinition("byte"),
                    ReadCollectionType.Array,
                    CollectionType.Array);
            }

            var elementSymbol = symbol.TryGetArrayElementSymbol();

            if (elementSymbol is null)
            {
                return null;
            }

            var elementDefinition = GetDefinition(elementSymbol);
            var readCollectionType = symbol.GetReadCollectionType();
            var collectionType = symbol.GetCollectionType();

            return new ArrayDefinition(
                type,
                SchemaType.Array,
                elementDefinition,
                readCollectionType,
                collectionType);
        }

        private IDefinition TryGetDictionaryDefinition(ITypeSymbol symbol)
        {
            if (!symbol.TryGetDictionarySymbols(out var keySymbol, out var valueSymbol))
            {
                return null;
            }

            var keyDefinition = GetDefinition(keySymbol);
            var valueDefinition = GetDefinition(valueSymbol);
            var dictionaryType = GetDictionaryType(symbol.Name);

            return new DictionaryDefinition(
                symbol.GetTypeName(),
                SchemaType.Dictionary,
                keyDefinition,
                valueDefinition,
                dictionaryType);
        }

        private static DictionaryType GetDictionaryType(string name)
        {
            if (name == "Dictionary")
            {
                return DictionaryType.Dictionary;
            }

            if (name == "IDictionary")
            {
                return DictionaryType.IDictionary;
            }

            if (name == "IReadOnlyDictionary")
            {
                return DictionaryType.IReadOnlyDictionary;
            }

            throw new ArgumentOutOfRangeException($"Cannot handle '{name}'.", nameof(name), null);
        }

        private IDefinition TryGetTuple2Definition(ITypeSymbol symbol)
        {
            if (!symbol.TryGetTuple2Symbols(out var item1Symbol, out var item2Symbol))
            {
                return null;
            }

            var item1Definition = GetDefinition(item1Symbol);
            var item2Definition = GetDefinition(item2Symbol);
            var schemaType = symbol.IsNullable() ? SchemaType.NullableTuple2 : SchemaType.Tuple2;

            return new Tuple2Definition(symbol.GetTypeName(), schemaType, item1Definition, item2Definition);
        }

        private IDefinition TryGetTuple3Definition(ITypeSymbol symbol)
        {
            if (!symbol.TryGetTuple3Symbols(out var item1Symbol, out var item2Symbol, out var item3Symbol))
            {
                return null;
            }

            var item1Definition = GetDefinition(item1Symbol);
            var item2Definition = GetDefinition(item2Symbol);
            var item3Definition = GetDefinition(item3Symbol);
            var schemaType = symbol.IsNullable() ? SchemaType.NullableTuple3 : SchemaType.Tuple3;

            return new Tuple3Definition(
                symbol.GetTypeName(),
                schemaType,
                item1Definition,
                item2Definition,
                item3Definition);
        }

        private IDefinition TryGetEnumDefinition(INamedTypeSymbol symbol)
        {
            if (!TryGetEnumUnderlyingType(symbol, out var underlyingType))
            {
                return null;
            }

            var underlyingDefinition = NativeDefinition.GetNativeDefinition(underlyingType.GetTypeName());

            return new EnumDefinition(symbol.GetTypeName(), underlyingDefinition);
        }

        private static bool TryGetEnumUnderlyingType(INamedTypeSymbol symbol, out INamedTypeSymbol enumUnderlyingType)
        {
            INamedTypeSymbol enumSymbol;

            if (symbol.TypeKind == TypeKind.Enum)
            {
                enumSymbol = symbol;
            }
            else if (symbol.UnwrapNullable(out var unwrappedSymbol) && unwrappedSymbol.TypeKind == TypeKind.Enum)
            {
                enumSymbol = unwrappedSymbol;
            }
            else
            {
                enumUnderlyingType = null;
                return false;
            }

            enumUnderlyingType = enumSymbol.EnumUnderlyingType;
            return true;
        }
    }
}
