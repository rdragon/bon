using System;
using System.Collections.Generic;

namespace Bon.SourceGeneration.Definitions
{
    partial class NativeDefinition
    {
        private static readonly Dictionary<string, NativeDefinition> _definitions = new Dictionary<string, NativeDefinition>();

        static NativeDefinition()
        {
            // Bookmark 659516266 (native serialization)
            // For every native type we add a definition.
            // This is the same set of types as can be found at bookmark 293228595.
            Add("string", SchemaType.String, false);
            Add("bool", SchemaType.Byte);
            Add("byte", SchemaType.Byte);
            Add("sbyte", SchemaType.SByte);
            Add("short", SchemaType.Short);
            Add("ushort", SchemaType.UShort);
            Add("int", SchemaType.Int);
            Add("uint", SchemaType.UInt);
            Add("long", SchemaType.Long);
            Add("ulong", SchemaType.ULong);
            Add("float", SchemaType.Float);
            Add("double", SchemaType.Double);
            Add("decimal", SchemaType.NullableDecimal);
            Add("System.Guid", schemaIdentifier: "ArraySchema.ByteArray");
            Add("char", SchemaType.WholeNumber);
            Add("System.DateTime", SchemaType.Long);
            Add("System.DateTimeOffset", SchemaType.Long);
            Add("System.TimeSpan", SchemaType.Long);
            Add("System.DateOnly", SchemaType.Int);
            Add("System.TimeOnly", SchemaType.Long);

            Add("bool?", SchemaType.WholeNumber);
            Add("byte?", SchemaType.WholeNumber);
            Add("sbyte?", SchemaType.SignedWholeNumber);
            Add("short?", SchemaType.SignedWholeNumber);
            Add("ushort?", SchemaType.WholeNumber);
            Add("int?", SchemaType.SignedWholeNumber);
            Add("uint?", SchemaType.WholeNumber);
            Add("long?", SchemaType.SignedWholeNumber);
            Add("ulong?", SchemaType.WholeNumber);
            Add("float?", SchemaType.FractionalNumber);
            Add("double?", SchemaType.FractionalNumber);
            Add("decimal?", SchemaType.FractionalNumber);
            Add("System.Guid?", schemaIdentifier: "ArraySchema.ByteArray");
            Add("char?", SchemaType.WholeNumber);
            Add("System.DateTime?", SchemaType.SignedWholeNumber);
            Add("System.DateTimeOffset?", SchemaType.SignedWholeNumber);
            Add("System.TimeSpan?", SchemaType.SignedWholeNumber);
            Add("System.DateOnly?", SchemaType.SignedWholeNumber);
            Add("System.TimeOnly?", SchemaType.SignedWholeNumber);
        }

        private static void Add(string type, SchemaType schemaType = default, bool isValueType = true, string schemaIdentifier = null)
        {
            schemaIdentifier = schemaIdentifier ?? $"NativeSchema.{schemaType}";
            _definitions[type] = new NativeDefinition(type, schemaType, isValueType, schemaIdentifier);
        }

        public static NativeDefinition GetNativeDefinition(string type) =>
            TryGetNativeDefinition(type) ??
            throw new ArgumentOutOfRangeException($"Cannot handle '{type}'.", nameof(type), null);

        public static NativeDefinition TryGetNativeDefinition(string type)
        {
            return _definitions.TryGetValue(type, out var definition) ? definition : null;
        }
    }
}
