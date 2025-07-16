using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration.DefinitionFactories
{
    internal sealed class RecordDefinitionFactory
    {
        private readonly DefinitionFactory _definitionFactory;

        public RecordDefinitionFactory(DefinitionFactory definitionFactory)
        {
            _definitionFactory = definitionFactory;
        }

        public RecordDefinition GetRecordDefinitionWithoutMembers(INamedTypeSymbol symbol)
        {
            var type = symbol.GetTypeName();
            var isConcreteType = symbol.TypeArguments.All(arg => !(arg is ITypeParameterSymbol));
            var isValueType = symbol.IsValueType;

            return new RecordDefinition(type, Array.Empty<Member>(), isValueType, false, isConcreteType);
        }

        public void AddMembers(RecordDefinition definition, INamedTypeSymbol symbol)
        {
            if (!definition.IsConcreteType)
            {
                return;
            }

            var members = GetMembers(symbol);
            var hasValidConstructor = SetConstructorIndices(symbol, members);

            definition.Members = members;
            definition.HasValidConstructor = hasValidConstructor;
        }

        /// <summary>
        /// Returns all the serializable members ordered by ID.
        /// </summary>
        private IReadOnlyList<Member> GetMembers(INamedTypeSymbol symbol)
        {
            symbol = symbol.UnwrapNullable() ?? symbol;

            RequireBonObjectAttribute(symbol);

            var members = symbol.GetMembers()
                .Select(TryGetMember)
                .Where(member => member != null)
                .OrderBy(member => member.Id)
                .ToArray();

            RequireUniqueIds(members, symbol);
            ForbidReservedIds(members, symbol);

            return members;
        }

        private static void RequireBonObjectAttribute(ISymbol symbol)
        {
            if (symbol.GetAttributes().All(attribute => attribute.AttributeClass?.Name != "BonObjectAttribute"))
            {
                throw new SourceGenerationException(
                    $"Type '{symbol}' must have a BonObjectAttribute.",
                    1761,
                    symbol);
            }
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
                    $"Cannot used reseved ID {badMember.Id} in type '{symbol}'.",
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
            var shouldIgnore = false;

            foreach (var attribute in attributes)
            {
                var name = attribute.AttributeClass?.Name;

                if (name == "BonMemberAttribute")
                {
                    var typedConstant = attribute.ConstructorArguments[0];
                    value = (int)typedConstant.Value;
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
                    $"Negative member IDs are not supported. Please give member '{symbol}' a non-negative ID.",
                    5347,
                    symbol);
            }

            if (value.HasValue && symbol.DeclaredAccessibility != Accessibility.Public)
            {
                throw new SourceGenerationException(
                    $"The BonMember attribute can only be used on public members. Please make '{symbol}' public or remove the attribute.",
                    3348,
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

            if (symbol.DeclaredAccessibility != Accessibility.Public)
            {
                return null;
            }

            throw new SourceGenerationException(
                $"Public member '{symbol}' must have a BonMember or BonIgnore attribute.",
                3963,
                symbol);
        }

        /// <summary>
        /// Sets <see cref="Member.ConstructorIndex"/> for each member.
        /// </summary>
        private bool SetConstructorIndices(INamedTypeSymbol symbol, IReadOnlyList<Member> members)
        {
            var hasEmptyConstructor = false;
            var dictionary = members.ToDictionary(member => member.Name, StringComparer.OrdinalIgnoreCase);
            symbol = symbol.UnwrapNullable() ?? symbol;

            foreach (var parameters in symbol.InstanceConstructors
                .Where(constructor =>
                    constructor.DeclaredAccessibility == Accessibility.Public ||
                    constructor.DeclaredAccessibility == Accessibility.Internal)
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

            SetConstructorIndicesForFactoryMethod(members, symbol);

            return false;
        }

        private static void SetConstructorIndicesForFactoryMethod(IReadOnlyList<Member> members, ISymbol symbol)
        {
            var counter = 0;

            foreach (var member in members)
            {
                if (!member.HasSetter)
                {
                    throw new SourceGenerationException($"Member '{member.Name}' of class '{symbol}' does not have a setter.", 6172, symbol);
                }

                member.ConstructorIndex = counter++;
            }
        }
    }
}
