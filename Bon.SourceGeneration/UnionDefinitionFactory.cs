using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal sealed class UnionDefinitionFactory
    {
        private readonly DefinitionFactory _definitionFactory;

        public UnionDefinitionFactory(DefinitionFactory definitionFactory)
        {
            _definitionFactory = definitionFactory;
        }

        public UnionDefinition GetUnionDefinition(INamedTypeSymbol symbol)
        {
            var id = symbol.GetTypeName();
            var isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
            var members = new List<UnionMember>();

            foreach (var attribute in GetBonIncludeAttributes(symbol).OrderBy(attribute => attribute.MemberId))
            {
                var definition = _definitionFactory.GetDefinition(attribute.Symbol);
                members.Add(new UnionMember(attribute.MemberId, definition));
            }

            if (members.Count == 0)
            {
                throw new SourceGenerationException(
                    $"The {GetName(symbol)} must have at least one BonInclude attribute.",
                    8876,
                    symbol);
            }

            RequireUniqueIds(members, symbol);
            RequireUniqueTypes(members, symbol);

            return new UnionDefinition(id, SchemaType.Union, isNullable, members);
        }

        private static void RequireUniqueIds(IReadOnlyList<UnionMember> members, ITypeSymbol symbol)
        {
            for (int i = 0; i < members.Count - 1; i++)
            {
                if (members[i].Id == members[i + 1].Id)
                {
                    throw new SourceGenerationException(
                        $"Multiple members with ID {members[i].Id} for {GetName(symbol)} found.",
                        8958,
                        symbol);
                }
            }
        }

        private static void RequireUniqueTypes(IReadOnlyList<UnionMember> members, ITypeSymbol symbol)
        {
            var types = new HashSet<string>();

            if (members.Select(member => member.Definition.Type).Where(type => !types.Add(type)).TryGetFirst(out var duplicateType))
            {
                throw new SourceGenerationException(
                    $"Multiple IDs for type '{duplicateType}' of {GetName(symbol)} found.",
                    3183,
                    symbol);
            }
        }

        private static IEnumerable<BonIncludeAttribute> GetBonIncludeAttributes(INamedTypeSymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                var name = attribute.AttributeClass?.Name;

                if (name == "BonIncludeAttribute")
                {
                    var memberId = (int)attribute.ConstructorArguments[0].Value;
                    var type = (ITypeSymbol)attribute.ConstructorArguments[1].Value;

                    if (memberId < 0)
                    {
                        // Negative member IDs result in larger message sizes.
                        throw new SourceGenerationException(
                            $"Type '{symbol}' has a BonInclude attribute with a negative ID. This is not supported.",
                            8275,
                            symbol);
                    }

                    if ((type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct) ||
                        type.IsAbstract ||
                        !IsAssignableTo(type, symbol))
                    {
                        throw new SourceGenerationException(
                            $"The type '{type}' must be a non-abstract class or struct implementing '{symbol}'.",
                            3974,
                            type);
                    }

                    yield return new BonIncludeAttribute(memberId, type);
                }
            }
        }

        private static bool IsAssignableTo(ITypeSymbol source, ITypeSymbol target)
        {
            if (target.TypeKind == TypeKind.Interface)
            {
                return source.AllInterfaces.Any(interfaceSymbol => SymbolEqualityComparer.Default.Equals(interfaceSymbol, target));
            }

            return Inherits(source, target);
        }

        private static bool Inherits(ITypeSymbol symbol, ITypeSymbol baseSymbol)
        {
            while (symbol != null)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol, baseSymbol))
                {
                    return true;
                }

                symbol = symbol.BaseType;
            }

            return false;
        }

        private sealed class BonIncludeAttribute
        {
            public int MemberId { get; }
            public ITypeSymbol Symbol { get; }

            public BonIncludeAttribute(int memberId, ITypeSymbol symbol)
            {
                MemberId = memberId;
                Symbol = symbol;
            }
        }

        private static string GetName(ITypeSymbol symbol)
        {
            return $"{GetKindName(symbol)} '{symbol}'";
        }

        private static string GetKindName(ITypeSymbol symbol)
        {
            return symbol.TypeKind == TypeKind.Interface ? "interface" : "abstract class";
        }
    }
}
