using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration
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
            var type = symbol.GetTypeName();

            if (_ancestors.TryGetValue(type, out var ancestor))
            {
                return ancestor;
            }

            var isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;

            if (NativeDefinition.TryGetNativeDefinition(type) is NativeDefinition nativeDefinition)
            {
                return nativeDefinition;
            }

            if (symbol.TryGetDictionarySymbols(out var keySymbol, out var valueSymbol))
            {
                return GetDictionaryDefinition(symbol, keySymbol, valueSymbol, isNullable);
            }

            if (symbol.TryGetArrayElementSymbol() is ITypeSymbol arrayElementSymbol)
            {
                return GetArrayDefinition(symbol, arrayElementSymbol, isNullable);
            }

            if (symbol.TryGetTuple2Symbols(out var item1Symbol, out var item2Symbol))
            {
                return GetTuple2Definition(symbol, item1Symbol, item2Symbol, isNullable);
            }

            if (symbol.TryGetTuple3Symbols(out var item1, out var item2, out var item3))
            {
                return GetTuple3Definition(symbol, item1, item2, item3, isNullable);
            }

            var named = symbol as INamedTypeSymbol ?? throw new SourceGenerationException($"Cannot handle '{symbol}'.", 8468, symbol);

            if (symbol.TypeKind == TypeKind.Enum)
            {
                return GetEnumDefinition(named, false);
            }

            if (WeakDefinition.TryGetWeakDefinition(type) is Definition definition)
            {
                return definition;
            }

            if (named.TryGetUnderlyingTypeFromNullableType() is INamedTypeSymbol underlyingSymbol &&
                underlyingSymbol.TypeKind == TypeKind.Enum)
            {
                return GetEnumDefinition(underlyingSymbol, true);
            }

            if (symbol.TypeKind == TypeKind.Interface || symbol.TypeKind == TypeKind.Class && symbol.IsAbstract)
            {
                return _unionDefinitionFactory.GetUnionDefinition(named);
            }

            return GetRecordDefinition(named);
        }

        private RecordDefinition GetRecordDefinition(INamedTypeSymbol named)
        {
            var type = named.GetTypeName();
            var recordDefinition = _recordDefinitionFactory.GetRecordDefinitionWithoutMembers(named);

            _ancestors.Add(type, recordDefinition);
            _recordDefinitionFactory.AddMembers(recordDefinition, named);
            _ancestors.Remove(type);

            return recordDefinition;
        }

        private ArrayDefinition GetArrayDefinition(ITypeSymbol collectionSymbol, ITypeSymbol elementSymbol, bool isNullable)
        {
            var elementDefinition = GetDefinition(elementSymbol);
            var readCollectionType = collectionSymbol.GetReadCollectionType();
            var collectionType = collectionSymbol.GetCollectionType();

            return new ArrayDefinition(
                collectionSymbol.GetTypeName(),
                SchemaType.Array,
                isNullable,
                elementDefinition,
                readCollectionType,
                collectionType);
        }

        private DictionaryDefinition GetDictionaryDefinition(
            ITypeSymbol collectionSymbol,
            ITypeSymbol keySymbol,
            ITypeSymbol valueSymbol,
            bool isNullable)
        {
            var keyDefinition = GetDefinition(keySymbol);
            var valueDefinition = GetDefinition(valueSymbol);
            var name = collectionSymbol.Name;
            var dictionaryType = GetDictionaryType(name);

            return new DictionaryDefinition(
                collectionSymbol.GetTypeName(),
                SchemaType.Dictionary,
                isNullable,
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

        private Tuple2Definition GetTuple2Definition(ITypeSymbol symbol, ITypeSymbol item1Symbol, ITypeSymbol item2Symbol, bool isNullable)
        {
            var item1Definition = GetDefinition(item1Symbol);
            var item2Definition = GetDefinition(item2Symbol);

            return new Tuple2Definition(symbol.GetTypeName(), SchemaType.Tuple2, isNullable, item1Definition, item2Definition);
        }

        private Tuple3Definition GetTuple3Definition(
            ITypeSymbol symbol,
            ITypeSymbol item1Symbol,
            ITypeSymbol item2Symbol,
            ITypeSymbol item3Symbol,
            bool isNullable)
        {
            var item1Definition = GetDefinition(item1Symbol);
            var item2Definition = GetDefinition(item2Symbol);
            var item3Definition = GetDefinition(item3Symbol);

            return new Tuple3Definition(
                symbol.GetTypeName(),
                SchemaType.Tuple3,
                isNullable,
                item1Definition,
                item2Definition,
                item3Definition);
        }

        private IDefinition GetEnumDefinition(INamedTypeSymbol enumSymbol, bool isNullable)
        {
            var type = enumSymbol.GetTypeName() + (isNullable ? "?" : "");
            var underlyingType = enumSymbol.EnumUnderlyingType;

            var underlyingDefinition =
                (GetDefinition(underlyingType) as NativeDefinition)?.ChangeNullability(isNullable) ??
                throw new SourceGenerationException($"Cannot handle underlying type '{underlyingType}' of '{enumSymbol}'.", 2184, enumSymbol);

            return new EnumDefinition(type, underlyingDefinition.SchemaType, isNullable, underlyingDefinition);
        }
    }
}
