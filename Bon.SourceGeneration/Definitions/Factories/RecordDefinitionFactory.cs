using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.Definitions.Factories
{
    internal sealed class RecordDefinitionFactory
    {
        private readonly DefinitionFactory _definitionFactory;

        public RecordDefinitionFactory(DefinitionFactory definitionFactory)
        {
            _definitionFactory = definitionFactory;
        }

        public RecordDefinition GetRecordDefinitionWithoutMembers(SymbolInfo symbolInfo)
        {
            var symbol = symbolInfo.Symbol;
            var isConcreteType = symbolInfo.TypeArguments.All(arg => !(arg is ITypeParameterSymbol));
            var isValueType = symbol.IsValueType;
            var isNullable = symbolInfo.IsNullable;
            var hasOnDeserialized = HasOnDeserialized(symbolInfo);

            if (!symbol.IsAccessible())
            {
                throw new SourceGenerationException($"Type '{symbol}' should be public or internal.", 1124, symbol);
            }

            return new RecordDefinition(
                symbolInfo.Type,
                Array.Empty<Member>(),
                isValueType,
                false,
                hasOnDeserialized,
                isConcreteType,
                isNullable);
        }

        private static bool HasOnDeserialized(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol.GetMembers().Any(member =>
                member.Kind == SymbolKind.Method &&
                member.Name == "OnDeserialized" &&
                member.IsAccessible() &&
                member is IMethodSymbol method &&
                method.Parameters.Length == 0);
        }

        public void AddMembers(RecordDefinition definition, INamedTypeSymbol symbol)
        {
            if (!definition.IsConcreteType)
            {
                return;
            }

            var bonObjectAttribute = RequireBonObjectAttribute(symbol);
            var members = GetMembers(symbol);
            var hasValidConstructor = SetConstructorIndices(symbol, members, bonObjectAttribute);

            definition.Members = members;
            definition.HasValidConstructor = hasValidConstructor;
        }

        /// <summary>
        /// Returns all the serializable members ordered by ID.
        /// </summary>
        private IReadOnlyList<Member> GetMembers(INamedTypeSymbol symbol)
        {
            var members = symbol.GetMembers()
                .Select(TryGetMember)
                .Where(member => member != null)
                .OrderBy(member => member.Id)
                .ToArray();

            if (members.Length == 0)
            {
                members = new[] { Member.CreateVirtualMember() };
            }

            RequireUniqueIds(members, symbol);
            ForbidReservedIds(members, symbol);

            return members;
        }

        private static AttributeData RequireBonObjectAttribute(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes().Where(attribute => attribute.AttributeClass?.Name == "BonObjectAttribute");
            if (attributes.TryGetFirst(out var result))
            {
                return result;
            }

            throw new SourceGenerationException(
                $"Type '{symbol}' must have a BonObjectAttribute.",
                1761,
                symbol);
        }

        private static void RequireUniqueIds(IReadOnlyList<Member> members, ISymbol symbol)
        {
            for (int i = 0; i < members.Count - 1; i++)
            {
                if (members[i].Id == members[i + 1].Id)
                {
                    throw new SourceGenerationException(
                        $"Multiple members with ID {members[i].Id} found in type '{symbol}'.",
                        6948,
                        symbol);
                }
            }
        }

        private static void ForbidReservedIds(IReadOnlyList<Member> members, ISymbol symbol)
        {
            if (!symbol.GetAttributes()
                .Where(attribute => attribute.AttributeClass?.Name == "BonReservedMembersAttribute")
                .TryGetFirst(out var data) || data.ConstructorArguments.Length == 0)
            {
                return;
            }

            var reservedIds = new HashSet<int>(data.ConstructorArguments[0].Values.Select(typedConstant => (int)typedConstant.Value));

            if (members.Where(member => reservedIds.Contains(member.Id)).TryGetFirst(out var badMember))
            {
                throw new SourceGenerationException(
                    $"Cannot used reserved ID {badMember.Id} in type '{symbol}'.",
                    6697,
                    symbol);
            }
        }

        private Member TryGetMember(ISymbol symbol)
        {
            var propertySymbol = symbol as IPropertySymbol;
            var fieldSymbol = symbol as IFieldSymbol;

            if (propertySymbol is null && fieldSymbol is null)
            {
                return null;
            }

            if (!(TryGetMemberId(symbol) is int id))
            {
                return null;
            }

            var type = propertySymbol?.Type ?? fieldSymbol.Type;
            var definition = _definitionFactory.GetDefinition(type);
            var name = symbol.Name;
            var hasSetter = propertySymbol is null || propertySymbol.SetMethod != null;

            return new Member(name, id, definition, hasSetter);
        }

        private int? TryGetMemberId(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes();
            int? value = null;
            var shouldIgnore = true; // For now, ignore by default, to get rid of all those BonIgnore attributes.

            foreach (var attribute in attributes)
            {
                var name = attribute.AttributeClass?.Name;

                if (name == "BonMemberAttribute")
                {
                    var typedConstant = attribute.ConstructorArguments[0];
                    value = (int)typedConstant.Value;
                    shouldIgnore = false;
                }
                else if (name == "BonIgnoreAttribute")
                {
                    shouldIgnore = true;
                }
            }

            if (value.HasValue && shouldIgnore)
            {
                throw new SourceGenerationException(
                    $"Member '{symbol}' cannot have both a BonMember attribute and a BonIgnore attribute.",
                    9778,
                    symbol);
            }

            if (value < 0)
            {
                // Negative member IDs result in larger schema sizes.
                // Also the line at bookmark 541895765 assumes that the member ID is larger than int.MinValue.
                throw new SourceGenerationException(
                    $"Negative member IDs are not supported. Error in member '{symbol}'.",
                    5347,
                    symbol);
            }

            if (value.HasValue && !symbol.IsAccessible())
            {
                throw new SourceGenerationException(
                    $"The BonMember attribute can only be used on public or internal members. Error in member '{symbol}'.",
                    3348,
                    symbol);
            }

            if (shouldIgnore && ((symbol as IPropertySymbol)?.IsRequired == true || (symbol as IFieldSymbol)?.IsRequired == true))
            {
                throw new SourceGenerationException(
                    $"The 'required' keyword is not allowed on an ignored member. Error in member '{symbol}'.",
                    8791,
                    symbol);
            }

            if (value.HasValue || shouldIgnore)
            {
                return value;
            }

            // An auto-generated property in record types.
            if (symbol.Name == "EqualityContract")
            {
                return null;
            }

            if (!symbol.IsAccessible())
            {
                return null;
            }

            throw new SourceGenerationException(
                $"Public or internal member '{symbol}' must have a BonMember or BonIgnore attribute.",
                3963,
                symbol);
        }

        /// <summary>
        /// Sets <see cref="Member.ConstructorIndex"/> for each member.
        /// </summary>
        private bool SetConstructorIndices(INamedTypeSymbol symbol, IReadOnlyList<Member> members, AttributeData bonObjectAttribute)
        {
            var hasEmptyConstructor = false;
            var dictionary = members.ToDictionary(member => member.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var parameters in symbol.InstanceConstructors
                .Where(constructor => constructor.IsAccessible())
                .Select(constructor => constructor.Parameters))
            {
                var count = parameters.Length;
                var i = 0;

                hasEmptyConstructor |= count == 0;

                if (count != members.Count)
                {
                    continue;
                }

                for (; i < count; i++)
                {
                    var parameter = parameters[i];

                    if (!dictionary.TryGetValue(parameter.Name, out var member))
                    {
                        break;
                    }

                    if (parameter.Type.GetTypeName() != member.Definition.Type)
                    {
                        break;
                    }

                    member.ConstructorIndex = i;
                }

                if (i == count)
                {
                    return true;
                }
            }

            if (!hasEmptyConstructor)
            {
                throw new SourceGenerationException(
                    $"The class '{symbol}' does not have an accessible empty constructor nor a constructor with the correct " +
                    $"parameter names and types.",
                    2677,
                    symbol);
            }

            if (bonObjectAttribute.TryGetNamedArgumentValue<bool>("ForceNonEmptyConstructor"))
            {
                throw new SourceGenerationException(
                    $"The class '{symbol}' does not have an accessible constructor with the correct parameter names and types.",
                    2677,
                    symbol);
            }

            SetConstructorIndicesForFactoryMethod(members, symbol);

            return false;
        }

        private static void SetConstructorIndicesForFactoryMethod(IReadOnlyList<Member> members, ISymbol symbol)
        {
            var counter = 0;

            foreach (var member in members)
            {
                if (!member.HasSetter && !member.IsVirtual)
                {
                    throw new SourceGenerationException($"Member '{member.Name}' of class '{symbol}' does not have a setter.", 6172, symbol);
                }

                member.ConstructorIndex = counter++;
            }
        }
    }
}
