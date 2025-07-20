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
        /// //2at
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

        public static int AddHashesOf<T>(this int hash, IEnumerable<T> values) => values.Aggregate(hash, AddHashOf);

        public static Location TryGetLocation(this ISymbol symbol)
        {
            if (symbol.Locations.Length > 0)
            {
                return symbol.Locations[0];
            }

            return null;
        }

        public static bool IsNullable(this INamedTypeSymbol symbol)
        {
            return !symbol.IsValueType || symbol.NullableAnnotation == NullableAnnotation.Annotated;
        }
    }
}
