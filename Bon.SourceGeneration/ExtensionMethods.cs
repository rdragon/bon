using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bon.SourceGeneration
{
    internal static class ExtensionMethods
    {
        public static void AddEmptyLine(this List<string> lines)
        {
            if (lines.Count == 0)
            {
                return;
            }

            var lastLine = lines[lines.Count - 1];

            if (lastLine == "" || lastLine == "{")
            {
                return;
            }

            lines.Add("");
        }

        /// <summary>
        /// Returns "true" or "false".
        /// </summary>
        public static string ToStringLower(this bool b) => b ? "true" : "false";

        public static SymbolInfo GetSymbolInfo(this ITypeSymbol symbol)
        {
            var type = GetTypeName(symbol);
            var isNullable = symbol.IsReferenceType || symbol.NullableAnnotation == NullableAnnotation.Annotated;

            if (isNullable && symbol.IsValueType && symbol is INamedTypeSymbol named)
            {
                symbol = named.TypeArguments[0];
            }

            var fullName = symbol is IArrayTypeSymbol ? "System.Array" : $"{symbol.ContainingNamespace}.{symbol.MetadataName}";
            var typeArguments = GetTypeArguments(symbol);

            return new SymbolInfo
            {
                Symbol = symbol,
                IsNullable = isNullable,
                Type = type,
                FullName = fullName,
                TypeArguments = typeArguments,
            };
        }

        private static ImmutableArray<ITypeSymbol> GetTypeArguments(ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol array)
            {
                return ImmutableArray.Create(array.ElementType);
            }

            if (symbol is INamedTypeSymbol named)
            {
                return named.TypeArguments;
            }

            return ImmutableArray<ITypeSymbol>.Empty;
        }

        /// <summary>
        /// Returns the name of the type corresponding to the symbol.
        /// This is the name that is used in the generated code.
        /// For nullable reference types the name does not end with a question mark, as the generated code does
        /// not use the nullable reference type feature.
        /// Example return values: "int?", "string", "System.Collections.Generic.List<int>".
        /// </summary>
        public static string GetTypeName(this ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol arraySymbol)
            {
                return $"{GetTypeName(arraySymbol.ElementType)}[]";
            }

            if (symbol is INamedTypeSymbol named)
            {
                if (symbol.IsValueType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    return GetTypeName(named.TypeArguments[0]) + "?";
                }

                if (named.IsGenericType)
                {
                    var arguments = string.Join(", ", named.TypeArguments.Select(GetTypeName));

                    return symbol.IsTupleType ?
                        $"({arguments})" :
                        $"{symbol.ContainingNamespace}.{symbol.Name}<{arguments}>";
                }
            }

            return symbol.WithNullableAnnotation(NullableAnnotation.None).ToString();
        }


        public static void RequireAll<T>(this IEnumerable<T> enumerable, Func<T, bool> condition)
        {
            foreach (var item in enumerable)
            {
                if (!condition(item))
                {
                    throw new InvalidOperationException($"Element '{item}' does not satisfy the condition.");
                }
            }
        }

        public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, out T first)
        {
            var enumerator = enumerable.GetEnumerator();

            if (enumerator.MoveNext())
            {
                first = enumerator.Current;
                return true;
            }

            first = default;
            return false;
        }

        public static int AddHashOf<T>(this int hash, T value) => hash * 87977 + value.GetHashCode();

        public static Location TryGetLocation(this ISymbol symbol)
        {
            if (symbol.Locations.Length > 0)
            {
                return symbol.Locations[0];
            }

            return null;
        }

        public static bool IsNullable(this SchemaType schemaType)
        {
            // Bookmark 662349317
            return
                schemaType == SchemaType.WholeNumber ||
                schemaType == SchemaType.SignedWholeNumber ||
                schemaType == SchemaType.FractionalNumber ||
                schemaType == SchemaType.NullableRecord ||
                schemaType == SchemaType.NullableTuple2 ||
                schemaType == SchemaType.NullableTuple3 ||
                schemaType == SchemaType.Union ||
                schemaType == SchemaType.Array ||
                schemaType == SchemaType.Dictionary ||
                schemaType == SchemaType.String;
        }

        public static T TryGetNamedArgumentValue<T>(this AttributeData attributeData, string name)
        {
            var typedConstants = attributeData.NamedArguments.Where(pair => pair.Key == name).Select(pair => pair.Value);
            return typedConstants.TryGetFirst(out var typedConstant) ? (T)typedConstant.Value : default;
        }

        public static bool IsAccessible(this ISymbol symbol) =>
            symbol.DeclaredAccessibility == Accessibility.Public ||
            symbol.DeclaredAccessibility == Accessibility.Internal ||
            symbol.DeclaredAccessibility == Accessibility.ProtectedOrInternal;
    }
}
