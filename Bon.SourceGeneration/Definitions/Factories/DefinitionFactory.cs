using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions.Factories
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
            var symbolInfo = symbol.GetSymbolInfo();

            if (_ancestors.TryGetValue(symbolInfo.Type, out var ancestor))
            {
                return ancestor;
            }

            return
                TryGetArrayDefinition(symbolInfo) ??
                NativeDefinition.TryGetNativeDefinition(symbolInfo.Type) ??
                TryGetEnumDefinition(symbolInfo) ??
                TryGetDictionaryDefinition(symbolInfo) ??
                TryGetTuple2Definition(symbolInfo) ??
                TryGetTuple3Definition(symbolInfo) ??
                _unionDefinitionFactory.TryGetUnionDefinition(symbolInfo) ??
                GetRecordDefinition(symbolInfo);
        }

        private RecordDefinition GetRecordDefinition(SymbolInfo symbolInfo)
        {
            var recordDefinition = _recordDefinitionFactory.GetRecordDefinitionWithoutMembers(symbolInfo);

            _ancestors.Add(symbolInfo.Type, recordDefinition);
            _recordDefinitionFactory.AddMembers(recordDefinition, symbolInfo.GetNamedTypeSymbol());
            _ancestors.Remove(symbolInfo.Type);

            return recordDefinition;
        }

        private ArrayDefinition TryGetArrayDefinition(SymbolInfo symbolInfo)
        {
            if (!(symbolInfo.TryGetArrayInfo() is ArrayInfo arrayInfo))
            {
                return null;
            }

            var elementDefinition = GetDefinition(symbolInfo.TypeArguments[0]);

            return new ArrayDefinition(symbolInfo.Type, elementDefinition, arrayInfo.ReadCollectionType, arrayInfo.CollectionType);
        }

        private IDefinition TryGetDictionaryDefinition(SymbolInfo symbolInfo)
        {
            if (!(symbolInfo.TryGetDictionaryType() is DictionaryType dictionaryType))
            {
                return null;
            }

            var keyDefinition = GetDefinition(symbolInfo.TypeArguments[0]);
            var valueDefinition = GetDefinition(symbolInfo.TypeArguments[1]);

            return new DictionaryDefinition(
                symbolInfo.Type,
                keyDefinition,
                valueDefinition,
                dictionaryType);
        }

        private IDefinition TryGetTuple2Definition(SymbolInfo symbolInfo)
        {
            if (symbolInfo.FullName != "System.ValueTuple`2")
            {
                return null;
            }

            var item1Definition = GetDefinition(symbolInfo.TypeArguments[0]);
            var item2Definition = GetDefinition(symbolInfo.TypeArguments[1]);
            var schemaType = symbolInfo.IsNullable ? SchemaType.NullableTuple2 : SchemaType.Tuple2;

            return new Tuple2Definition(symbolInfo.Type, schemaType, item1Definition, item2Definition);
        }

        private IDefinition TryGetTuple3Definition(SymbolInfo symbolInfo)
        {
            if (symbolInfo.FullName != "System.ValueTuple`3")
            {
                return null;
            }

            var item1Definition = GetDefinition(symbolInfo.TypeArguments[0]);
            var item2Definition = GetDefinition(symbolInfo.TypeArguments[1]);
            var item3Definition = GetDefinition(symbolInfo.TypeArguments[2]);
            var schemaType = symbolInfo.IsNullable ? SchemaType.NullableTuple3 : SchemaType.Tuple3;

            return new Tuple3Definition(
                symbolInfo.Type,
                schemaType,
                item1Definition,
                item2Definition,
                item3Definition);
        }

        private IDefinition TryGetEnumDefinition(SymbolInfo symbolInfo)
        {
            var underlyingSymbol = symbolInfo.TryGetEnumUnderlyingSymbol();

            if (underlyingSymbol is null)
            {
                return null;
            }

            var underlyingDefinition = NativeDefinition.GetNativeDefinition(underlyingSymbol.GetTypeName());

            if (symbolInfo.IsNullable)
            {
                underlyingDefinition = underlyingDefinition.SwapNullability();
            }

            return new EnumDefinition(symbolInfo.Type, underlyingDefinition);
        }
    }
}
