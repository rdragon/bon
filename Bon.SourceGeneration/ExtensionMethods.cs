using Bon.SourceGeneration.Definitions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns the read collection type of the given collection type symbol.
        /// </summary>
        public static ReadCollectionType GetReadCollectionType(this ITypeSymbol collectionSymbol)
        {
            if (collectionSymbol is IArrayTypeSymbol)
            {
                return ReadCollectionType.Array;
            }

            var name = collectionSymbol.Name;

            switch (name)
            {
                case "List":
                case "IList":
                case "ICollection":
                    return ReadCollectionType.List;

                case "IReadOnlyList":
                case "IReadOnlyCollection":
                case "IEnumerable":
                    return ReadCollectionType.Array;

                default:
                    throw new ArgumentOutOfRangeException(nameof(collectionSymbol), collectionSymbol, null);
            }
        }

        /// <summary>
        /// Returns the simplified collection type of the given collection type symbol.
        /// </summary>
        public static CollectionType GetCollectionType(this ITypeSymbol collectionSymbol)
        {
            if (collectionSymbol is IArrayTypeSymbol)
            {
                return CollectionType.Array;
            }

            var name = collectionSymbol.Name;

            switch (name)
            {
                case "List":
                case "IList":
                case "IReadOnlyList":
                    return CollectionType.List;

                case "ICollection":
                case "IReadOnlyCollection":
                case "IEnumerable":
                    return CollectionType.IEnumerable;

                default:
                    throw new ArgumentOutOfRangeException(nameof(collectionSymbol), collectionSymbol, null);
            }
        }

        /// <summary>
        /// Returns the element type symbol of a collection type symbol, or null if the collection type symbol is not a
        /// collection after all.
        /// </summary>
        public static ITypeSymbol TryGetArrayElementSymbol(this ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol array)
            {
                return array.ElementType;
            }

            if (symbol is INamedTypeSymbol named && named.IsGenericType)
            {
                var name = symbol.Name;

                if (name == "IReadOnlyList" ||
                    name == "List" ||
                    name == "IList" ||
                    name == "IEnumerable" ||
                    name == "ICollection" ||
                    name == "IReadOnlyCollection")
                {
                    return named.TypeArguments[0];
                }
            }

            return null;
        }

        public static bool TryGetDictionarySymbols(this ITypeSymbol symbol, out ITypeSymbol keySymbol, out ITypeSymbol valueSymbol)
        {
            if (symbol is INamedTypeSymbol named && named.IsGenericType)
            {
                var name = symbol.Name;

                if (name == "Dictionary" ||
                    name == "IDictionary" ||
                    name == "IReadOnlyDictionary")
                {
                    keySymbol = named.TypeArguments[0];
                    valueSymbol = named.TypeArguments[1];
                    return true;
                }
            }

            keySymbol = null;
            valueSymbol = null;
            return false;
        }

        public static bool TryGetTuple2Symbols(this ITypeSymbol symbol, out ITypeSymbol item1Symbol, out ITypeSymbol item2Symbol)
        {
            if (symbol is INamedTypeSymbol named)
            {
                named = named.UnwrapNullable() ?? named;

                if (named.IsGenericType)
                {
                    var name = named.Name;

                    if (name == "ValueTuple" && named.TypeArguments.Length == 2)
                    {
                        item1Symbol = named.TypeArguments[0];
                        item2Symbol = named.TypeArguments[1];
                        return true;
                    }
                }
            }

            item1Symbol = null;
            item2Symbol = null;
            return false;
        }

        public static bool TryGetTuple3Symbols(this ITypeSymbol symbol, out ITypeSymbol item1Symbol, out ITypeSymbol item2Symbol, out ITypeSymbol item3Symbol)
        {
            if (symbol is INamedTypeSymbol named)
            {
                named = named.UnwrapNullable() ?? named;

                if (named.IsGenericType)
                {
                    var name = named.Name;

                    if (name == "ValueTuple" && named.TypeArguments.Length == 3)
                    {
                        item1Symbol = named.TypeArguments[0];
                        item2Symbol = named.TypeArguments[1];
                        item3Symbol = named.TypeArguments[2];
                        return true;
                    }
                }
            }

            item1Symbol = null;
            item2Symbol = null;
            item3Symbol = null;
            return false;
        }

        /// <summary>
        /// Returns a value like "ExampleNamespace.ExampleClass?" or "int".
        /// </summary>
        public static string GetTypeName(this ITypeSymbol symbol) => symbol.ToString();

        //1at: maak dat ie altijd iets teruggeeft? dat ie dus zelfde werkt als functie in serializer.
        public static INamedTypeSymbol UnwrapNullable(this INamedTypeSymbol symbol)
        {
            return symbol.UnwrapNullable(out var type) ? type : null;
        }

        public static bool IsNullable(this ITypeSymbol symbol) => symbol.NullableAnnotation == NullableAnnotation.Annotated;

        /// <summary>
        /// //2at
        /// </summary>
        public static ITypeSymbol UpdateNullability(this ITypeSymbol symbol) =>
            symbol.IsValueType ? symbol : symbol.WithNullableAnnotation(NullableAnnotation.Annotated);

        public static INamedTypeSymbol ToNamedTypeSymbol(this ITypeSymbol symbol) =>
            symbol as INamedTypeSymbol ?? throw new SourceGenerationException($"Cannot handle '{symbol}'.", 8468, symbol);

        public static bool UnwrapNullable(this INamedTypeSymbol symbol, out INamedTypeSymbol result)
        {
            if (symbol.IsValueType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
            {
                result = (INamedTypeSymbol)symbol.TypeArguments[0];
                return true;
            }

            result = null;
            return false;
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
